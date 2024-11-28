using BuzzerWolf.Models;
using BuzzerWolf.Models.Extensions;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BuzzerWolf.ViewModels
{
    public partial class AutoPromotionViewModel : ObservableObject
    {
        private readonly BuzzerWolfContext _context;
        private readonly SynchronizationViewModel _syncViewModel;
        public AutoPromotionViewModel(BuzzerWolfContext context, SynchronizationViewModel syncViewModel)
        {
            _context = context;
            _syncViewModel = syncViewModel;
        }

        [ObservableProperty]
        private List<Season> seasons = new();
        [ObservableProperty]
        private Season? selectedSeason;

        [ObservableProperty]
        private List<Country> countries = new();
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(CanSelectDivision), nameof(Divisions))]
        private Country? selectedCountry;

        public List<int> Divisions => (SelectedCountry != null && SelectedCountry.Divisions > 1) ? Enumerable.Range(2, (int)(SelectedCountry.Divisions - 1)).ToList() : new List<int>();
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(ShowPromotionStandings))]
        private int? selectedDivision;

        [ObservableProperty]
        private List<PromotionStanding> promotionStandings = new();
        public bool ShowPromotionStandings => SelectedDivision != null;

        [ObservableProperty]
        private int totalPromotionSpots = 0;
        [ObservableProperty]
        private int autoPromotionSpots = 0;
        [ObservableProperty]
        private int botPromotionSpots = 0;
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(ShowChampionPromotionSpots))]
        private int championPromotionSpots = 0;

        public bool ShowChampionPromotionSpots => ChampionPromotionSpots > 0;

        partial void OnSelectedSeasonChanged(Season? value)
        {
            Task.Run(UpdatePromotionStandings);
        }


        partial void OnSelectedCountryChanged(Country? value)
        {
            Task.Run(UpdatePromotionStandings);
        }

        partial void OnSelectedDivisionChanged(int? value)
        {
            Task.Run(UpdatePromotionStandings);
        }

        private async Task UpdatePromotionStandings()
        {
            if (SelectedSeason == null || SelectedCountry == null || SelectedDivision == null) return;

            await _syncViewModel.SyncStandingsForDivision(false, SelectedCountry.Id, (int)SelectedDivision, SelectedSeason.Id);

            var auto = 0;
            var total = 0;
            var bot = 0;

            var leaguesList = _context.Leagues.Where(l => l.CountryId == SelectedCountry.Id && l.DivisionLevel == SelectedDivision).ToList();
            var promotionStandings = new List<PromotionStanding>();
            foreach (var league in leaguesList)
            {
                var leagueResults = _context.LeagueResults.FirstOrDefault(r => r.LeagueId == league.Id && r.Season == SelectedSeason.Id);
                var leagueStandings = _context.Standings.Where(s => s.LeagueId == league.Id && s.Season == SelectedSeason.Id).ToList();
                if (leagueResults is not null)
                {
                    if (_context.Standings.First(s => s.TeamId == leagueResults.Winner).IsBot)
                        bot++;

                    // Show winner and conference champions
                    var promotableTeams = leagueStandings.Where(s => s.TeamId == leagueResults.Winner || s.ConferenceRank == 1).Select(s => new PromotionStanding(s, league.Name)).ToList();
                    promotionStandings.AddRange(promotableTeams);

                    SetTeamChampionStatus(promotableTeams, league.Id, SelectedSeason.Id);
                }
                else
                {
                    if (_context.LeaguePlayoffs.Any(lp => lp.LeagueId == league.Id && lp.Season == SelectedSeason.Id))
                    {
                        // In the playoffs, no need to show anything other than conference champions in output
                        var promotableTeams = leagueStandings.Where(s => s.ConferenceRank == 1).Select(s => new PromotionStanding(s, league.Name)).ToList();
                        promotionStandings.AddRange(promotableTeams);

                        SetTeamEliminationStatus(promotableTeams, league.Id, SelectedSeason.Id);
                        await SetTeamScheduleStatus(promotableTeams, SelectedSeason.Id);
                        SetTeamChampionStatus(promotableTeams, league.Id, SelectedSeason.Id);
                    }
                    else
                    {
                        // Not in playoffs yet, include second place teams
                        var promotableTeams = leagueStandings.Where(s => s.ConferenceRank <= 2).Select(s => new PromotionStanding(s, league.Name)).ToList();
                        promotionStandings.AddRange(promotableTeams);

                        await SetTeamScheduleStatus(promotableTeams, SelectedSeason.Id);
                    }
                }
            }

            var nextDivisionHigher = (int)SelectedDivision - 1;
            var checkDivision = nextDivisionHigher;
            do
            {
                await _syncViewModel.SyncStandingsForDivision(false, SelectedCountry.Id, checkDivision, SelectedSeason.Id);
                var promotingLeaguesList = _context.Leagues.Where(l => l.CountryId == SelectedCountry.Id && l.DivisionLevel == checkDivision).ToList();
                foreach (var league in promotingLeaguesList)
                {
                    var leagueStandings = _context.Standings.Where(s => s.LeagueId == league.Id && s.Season == SelectedSeason.Id).ToList();
                    int leagueBots = (checkDivision == nextDivisionHigher)
                        ? leagueStandings.Where(s => s.ConferenceRank < 8 && s.IsBot).Count()
                        : leagueStandings.Where(s => s.IsBot).Count();

                    if (checkDivision == nextDivisionHigher)
                    {
                        auto += (SelectedCountry.Name == "Utopia" && SelectedDivision < 4 ? 2 :
                                 SelectedCountry.Name == "Utopia" && SelectedDivision == 4 ? 0 : 1);
                        total += (SelectedCountry.Name == "Utopia" ? 6 : 5);
                    }

                    total += leagueBots;
                    bot += leagueBots;
                }
                AutoPromotionSpots = auto;
                TotalPromotionSpots = total;
                BotPromotionSpots = bot;

                checkDivision--;
            } while (checkDivision >= 1);

            ChampionPromotionSpots = promotionStandings.Count(s => s.IsChampionPromotion);
            PromotionStandings = promotionStandings.OrderByDescending(s => s.IsChampionPromotion)
                                                   .ThenBy(s => s.ConferenceRank)
                                                   .ThenByDescending(s => s.Wins)
                                                   .ThenByDescending(s => s.PointDifference)
                                                   .ToList();

            foreach (var standing in PromotionStandings.Select((ps, idx) => new { Team = ps, Index = idx }))
            {
                standing.Team.PromotionRank = standing.Index + 1;
                standing.Team.IsAutoPromotion = standing.Index < AutoPromotionSpots + ChampionPromotionSpots;
                standing.Team.IsBotPromotion = standing.Index < BotPromotionSpots + AutoPromotionSpots + ChampionPromotionSpots;
                standing.Team.IsTotalPromotion = standing.Index < TotalPromotionSpots;
            }
        }

        private void SetTeamEliminationStatus(IEnumerable<PromotionStanding> promotableTeams, int leagueId, int season)
        {
            var leaguePlayoffMatches = _context.LeaguePlayoffs.Where(p => p.LeagueId == leagueId && p.Season == season);
            if (!leaguePlayoffMatches.Any())
                return;

            var matches = _context.Matches.Where(m => leaguePlayoffMatches.Any(po => po.MatchId == m.Id));
            foreach (var team in promotableTeams)
            {
                team.IsEliminated = !matches.Any(m => m.TeamParticipatedInMatch(team.TeamId)) ||
                                     matches.Any(m => m.Type != MatchType.Final && m.HasResult() && m.TeamLostMatch(team.TeamId)) ||
                                     matches.Where(m => m.Type == MatchType.Final && m.HasResult() && m.TeamLostMatch(team.TeamId)).Count() == 2;
            }
        }

        private async Task SetTeamScheduleStatus(IEnumerable<PromotionStanding> promotableTeams, int season)
        {
            await _syncViewModel.SyncScheduleForTeams(false, promotableTeams.Select(t => t.TeamId), season);

            foreach (var team in promotableTeams)
            {
                var remainingMatches = _context.Matches.Where(m => m.WinningTeamId == null && 
                                                                   (m.AwayTeamId == team.TeamId || m.HomeTeamId == team.TeamId) &&
                                                                   m.Type <= MatchType.Relegation)
                                                       .OrderBy(m => m.StartTime)
                                                       .ToList();
                var nextMatch = remainingMatches.FirstOrDefault();
                if (nextMatch != null)
                {
                    var nextOpponentId = nextMatch.OpponentTeamId(team.TeamId);
                    var nextOpponent = _context.Standings.First(s => s.TeamId == nextOpponentId && s.Season == season);
                    team.NextOpponent = $"{(nextMatch.AwayTeamId == team.TeamId ? "@" : "v")} {nextOpponent.TeamName}";
                    var lastMeeting = _context.Matches.Where(m => m.WinningTeamId != null &&
                                                                  ((m.AwayTeamId == team.TeamId && m.HomeTeamId == nextOpponentId) || 
                                                                   (m.HomeTeamId == team.TeamId && m.AwayTeamId == nextOpponentId))
                                                            )
                                                      .OrderBy(m => m.StartTime).LastOrDefault();
                    team.NextOpponentLastResult = lastMeeting != null ? lastMeeting.ResultForTeam(team.TeamId) : string.Empty;
                }

                var remainingOpponents = remainingMatches.Select(m => m.AwayTeamId == team.TeamId ? m.HomeTeamId : m.AwayTeamId).Distinct();
                var remainingRecords = _context.Standings.Where(s => remainingOpponents.Any(o => o == s.TeamId) && s.Season == season).ToList()
                                                         .Aggregate(new { Wins = 0, Losses = 0 }, (sum, next) => new { Wins = sum.Wins + next.Wins, Losses = sum.Losses + next.Losses });
                team.RemainingStrengthOfSchedule = $"{remainingRecords.Wins} - {remainingRecords.Losses}";
            }
        }

        private class RemainingRecords()
        {
            public int Wins { get; set; } = 0;
            public int Losses { get; set; } = 0;
        }

        private void SetTeamChampionStatus(IEnumerable<PromotionStanding> promotableTeams, int leagueId, int season)
        {
            var leagueResult = _context.LeagueResults.FirstOrDefault(r => r.LeagueId == leagueId && r.Season == season);
            foreach (var team in promotableTeams)
            {
                team.IsChampionPromotion = leagueResult?.Winner == team.TeamId;
            }
        }

        public bool CanSelectDivision => SelectedCountry != null;

        public void Activate()
        {
            Countries = _context.Countries.ToList();
            Seasons = _context.Seasons.OrderByDescending(s => s.Id).ToList();
        }
    }
}
