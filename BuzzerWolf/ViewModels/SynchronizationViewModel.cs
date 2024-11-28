using BuzzerWolf.BBAPI;
using BuzzerWolf.Extensions;
using BuzzerWolf.Models;
using BuzzerWolf.Models.Extensions;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace BuzzerWolf.ViewModels
{
    public partial class SynchronizationViewModel : ObservableObject
    {
        private readonly IBBAPIClient _client;
        private readonly BuzzerWolfContext _context;
        private readonly ProfileSelectionViewModel _profile;

        public SynchronizationViewModel(IBBAPIClient client, BuzzerWolfContext context, ProfileSelectionViewModel profile)
        {
            _client = client;
            _context = context;
            _profile = profile;
            _profile.PropertyChanged += new PropertyChangedEventHandler(OnProfileSelectionChanged);
        }

        private void OnProfileSelectionChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SelectedProfile")
            {
                PopulateSyncList();
            }
            if (e.PropertyName == "IsLoggedIn")
            {
                if (_profile.IsLoggedIn)
                {
                    Task.Run(() => Sync(false));
                }
            }
        }

        private void PopulateSyncList()
        {
            SynchronizedTables.ReplaceRange(_context.Sync);
        }

        [ObservableProperty]
        private ObservableRangeCollection<Sync> synchronizedTables = new();
        [ObservableProperty]
        private string statusDetail = string.Empty;
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(StatusSummary))]
        [NotifyCanExecuteChangedFor(nameof(ForceSyncCommand))]
        private bool running = false;

        public string StatusSummary => Running ? "Synchronization running" : "Synchronization up-to-date";
        private bool CanForceSync => !Running;

        [RelayCommand(CanExecute = nameof(CanForceSync))]
        private async Task ForceSync()
        {
            await Sync(true);
        }

        private async Task Sync(bool force)
        {
            Running = true;
            StatusDetail = "Synchronization starting...";

            await SyncSeasons(force);
            await SyncCountries(force);
            await SyncLeagues(force);
            await SyncStandings(force);

            await FinalizeSync();
            StatusDetail = "Synchronization complete";
            Running = false;
        }

        private async Task FinalizeSync()
        {
            await _context.SaveChangesAsync();
            PopulateSyncList();
        }

        private Sync NextSyncForTable(DateTimeOffset nextAutoSync, string table, int entityId = -1, int? season = null) =>
            new Sync { DataTable = table, EntityId = entityId, Season = season, LastSync = DateTime.UtcNow, NextAutoSync = nextAutoSync };
        private bool ShouldSyncTable(bool force, string table, int entityId = -1, int? season = null) => force || TableHasNotSynced(table, entityId, season) || TableNeedsToSync(table, entityId, season);
        private bool TableHasNotSynced(string table, int entityId, int? season) => !SynchronizedTables.Any(t => t.DataTable == table && t.EntityId == entityId && t.Season == season);
        private bool TableNeedsToSync(string table, int entityId, int? season) => SynchronizedTables.Any(t => t.DataTable == table && t.EntityId == entityId && t.Season == season && t.NextAutoSync < DateTimeOffset.UtcNow);

        private readonly int seasonLength = 14 * 7;
        private async Task SyncSeasons(bool force)
        {
            if (!ShouldSyncTable(force, "seasons"))
                return;

            StatusDetail = "Synchronizing season list...";
            var seasonList = (await _client.GetSeasons()).Seasons.FromBBAPI();
            await _context.Seasons.UpsertRange(seasonList).RunAsync();
            var endOfCurrentSeason = seasonList.OrderBy(s => s.Id).Last().Start + TimeSpan.FromDays(seasonLength);
            await _context.Sync.Upsert(NextSyncForTable(endOfCurrentSeason, "seasons")).RunAsync();
            PopulateSyncList();
        }

        private async Task SyncCountries(bool force)
        {
            if (!ShouldSyncTable(force, "countries"))
                return;

            StatusDetail = "Synchronizing countries list...";
            await _context.Countries.UpsertRange((await _client.GetCountries()).FromBBAPI()).RunAsync();
            var nextCountrySync = _context.Sync.First(s => s.DataTable == "seasons").NextAutoSync;
            await _context.Sync.Upsert(NextSyncForTable(nextCountrySync, "countries")).RunAsync();
        }

        private async Task SyncLeagues(bool force)
        {
            if (!ShouldSyncTable(force, "leagues"))
                return;

            StatusDetail = "Synchronizing leagues list...";
            foreach (var country in _context.Countries)
            {
                for (int leagueLevel = 1; leagueLevel <= (country.Divisions ?? 1); leagueLevel++)
                {
                    await _context.Leagues.UpsertRange((await _client.GetLeagues(country.Id, leagueLevel)).FromBBAPI(country.Id, leagueLevel)).On(l => l.Id).RunAsync();
                }
            }
            var nextLeaguesListSync = _context.Sync.First(s => s.DataTable == "seasons").NextAutoSync;
            await _context.Sync.Upsert(NextSyncForTable(nextLeaguesListSync, "leagues")).RunAsync();
            PopulateSyncList();
        }

        private async Task SyncStandings(bool force)
        {
            var syncedLeagues = _context.Sync.Where(s => s.DataTable.StartsWith("standings")).Select(GetLeagueToSync);
            await SyncStandings(force, syncedLeagues);
        }

        private LeagueToSync GetLeagueToSync(Sync syncTableRecord)
        {
            var dataTableInfo = syncTableRecord.DataTable.Split("_");
            var league = int.Parse(dataTableInfo.ElementAt(1).Replace("league", ""));
            var season = int.Parse(dataTableInfo.ElementAt(2).Replace("season", ""));
            return new LeagueToSync(league, season);
        }

        public async Task SyncStandingsForDivision(bool force, int countryId, int divisionLevel, int season)
        {
            await SyncStandings(force, _context.Leagues.Where(l => l.CountryId == countryId && l.DivisionLevel == divisionLevel).Select(l => new LeagueToSync(l.Id, season)));
        }

        private async Task SyncStandings(bool force, IEnumerable<LeagueToSync> leaguesToSync)
        {
            StatusDetail = "Synchronizing league standings...";
            var leagueLoadTasks = new List<Task<StandingsLoadResult>>();
            foreach (var league in leaguesToSync)
            {
                if (ShouldSyncTable(force, "standings", league.League, league.Season))
                {
                    leagueLoadTasks.Add(LoadStandingsData(league.League, league.Season));
                }
            }

            var standings = await Task.WhenAll(leagueLoadTasks);

            var standingsToUpdate = standings.SelectMany(s => s.Standings);
            await _context.Standings.UpsertRange(standingsToUpdate).RunAsync();
            var resultsToUpdate = standings.Where(s => s.Results != null).Select(s => s.Results!);
            await _context.LeagueResults.UpsertRange(resultsToUpdate).RunAsync();
            var matchesToUpdate = standings.SelectMany(s => s.PlayoffMatches);
            await _context.Matches.UpsertRange(matchesToUpdate).RunAsync();
            var playoffScheduleToUpdate = standings.SelectMany(s => s.PlayoffMatches.Select(pm => new PlayoffSchedule
            {
                LeagueId = s.LeagueId,
                Season = s.Season,
                MatchId = pm.Id,
            }));
            await _context.LeaguePlayoffs.UpsertRange(playoffScheduleToUpdate).RunAsync();

            var nextStandingsSync = DateTimeOffset.UtcNow + TimeSpan.FromDays(DaysUntilNextLeagueMatch());
            var syncRecordsToUpdate = standings.Select(s => NextSyncForTable(nextStandingsSync, "standings", s.LeagueId, s.Season));
            await _context.Sync.UpsertRange(syncRecordsToUpdate).RunAsync();
            PopulateSyncList();
            StatusDetail = "Synchronization complete";
        }

        private class LeagueToSync
        {
            public LeagueToSync(int league, int season)
            {
                League = league;
                Season = season;
            }

            public int League { get; }
            public int Season { get; }
        }

        private int DaysUntilNextLeagueMatch()
        {
            var today = DateTime.Today;
            
            var daysUntilTuesday = ((int)DayOfWeek.Tuesday - (int)today.DayOfWeek + 7) % 7 + 1;
            var daysUntilSaturday = ((int)DayOfWeek.Saturday - (int)today.DayOfWeek + 7) % 7 + 1;
            return Math.Min(daysUntilTuesday, daysUntilSaturday);
        }

        private async Task<StandingsLoadResult> LoadStandingsData(int leagueId, int season)
        {
            var standings = await _client.GetStandings(leagueId, season);
            return new StandingsLoadResult
            {
                LeagueId = leagueId,
                Season = season,
                Standings = standings.FromBBAPI(),
                Results = ResultExtensions.FromBBAPI(standings),
                PlayoffMatches = MatchExtensions.FromBBAPI(standings),
            };
        }

        private class StandingsLoadResult
        {
            public int LeagueId { get; set; }
            public int Season { get; set; }
            public IEnumerable<Standings> Standings { get; set; }
            public Result? Results { get; set; }
            public IEnumerable<Match> PlayoffMatches { get; set; }
        }

        public async Task SyncScheduleForTeams(bool force, IEnumerable<int> teamsToSync, int season)
        {
            StatusDetail = "Synchronizing schedules";

            var scheduleLoadTasks = new List<Task<ScheduleLoadResult>>();
            foreach (var team in teamsToSync)
            {
                if (ShouldSyncTable(force, "schedule", team, season))
                {
                    scheduleLoadTasks.Add(LoadScheduleData(team, season));
                }
            }

            var schedules = await Task.WhenAll(scheduleLoadTasks);

            var matchesToUpdate = schedules.SelectMany(s => s.Matches);
            await _context.Matches.UpsertRange(matchesToUpdate).RunAsync();

            var nextScheduleSync = DateTimeOffset.UtcNow + TimeSpan.FromDays(DaysUntilNextLeagueMatch());
            var syncRecordsToUpdate = schedules.Select(s => NextSyncForTable(nextScheduleSync, "schedule", s.TeamId, s.Season));
            await _context.Sync.UpsertRange(syncRecordsToUpdate).RunAsync();

            PopulateSyncList();
            StatusDetail = "Synchronization complete";
        }

        private async Task<ScheduleLoadResult> LoadScheduleData(int team, int season)
        {
            var schedule = await _client.GetSchedule(team, season);
            return new ScheduleLoadResult
            {
                TeamId = team,
                Season = season,
                Matches = schedule.Matches.FromBBAPI()
            };
        }

        private class ScheduleLoadResult
        {
            public int TeamId { get; set; }
            public int Season { get; set; }
            public IEnumerable<Match> Matches { get; set; }
        }
    }
}