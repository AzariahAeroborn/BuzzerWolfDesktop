using BuzzerWolf.BBAPI;
using BuzzerWolf.BBAPI.Model;
using BuzzerWolf.Extensions;
using BuzzerWolf.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Data;

namespace BuzzerWolf.ViewModels
{
    public partial class AutoPromotionViewModel : ObservableObject
    {
        private readonly IBBAPIClient _bbapi;
        public AutoPromotionViewModel(IBBAPIClient bbapi)
        {
            _bbapi = bbapi;
        }

        [ObservableProperty]
        private int? season;

        [ObservableProperty]
        private List<Country> countries = new();
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(CanSelectDivision),nameof(Divisions))]
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

        partial void OnSeasonChanged(int? value)
        {
            OnSelectedDivisionChanged(null);
        }


        partial void OnSelectedCountryChanged(Country? value)
        {
            OnSelectedDivisionChanged(null);
        }

        partial void OnSelectedDivisionChanged(int? value)
        {
            if (SelectedCountry == null || SelectedDivision == null) return;

            var auto = 0;
            var total = 0;
            var bot = 0;

            var leaguesList = Task.Run(() => _bbapi.GetLeagues(SelectedCountry.Id, SelectedDivision.Value)).Result;
            var standings = new List<TeamStanding>();
            foreach (var league in leaguesList)
            {
                var leagueStandings = Task.Run(() => _bbapi.GetStandings(league.Id, Season)).Result;
                if (leagueStandings.IsFinal)
                {
                    var winner = leagueStandings.Big8.Where(t => t.IsWinner).Union(leagueStandings.Great8.Where(t => t.IsWinner)).First();
                    if (winner.IsBot) { bot++; }
                }
                standings.AddRange(leagueStandings.Big8);
                standings.AddRange(leagueStandings.Great8);
            }

            var nextDivisionHigher = (int)SelectedDivision - 1;
            var checkDivision = nextDivisionHigher;
            do
            {
                var promotingLeaguesList = Task.Run(() => _bbapi.GetLeagues(SelectedCountry.Id, checkDivision)).Result;
                foreach (var league in promotingLeaguesList)
                {
                    int leagueBots = 0;
                    var leagueStandings = Task.Run(() => _bbapi.GetStandings(league.Id)).Result;
                    if (checkDivision == nextDivisionHigher)
                    {
                        if (leagueStandings.IsFinal)
                        {

                        } else
                        {
                            leagueBots = leagueStandings.Big8.Count(t => t.ConferenceRank < 8 && t.IsBot) + leagueStandings.Great8.Count(t => t.ConferenceRank < 8 && t.IsBot);
                        }
                    } else
                    {
                        leagueBots = leagueStandings.Big8.Count(t => t.IsBot) + leagueStandings.Great8.Count(t => t.IsBot);
                    }

                    if (checkDivision == nextDivisionHigher)
                    {
                        auto += (SelectedCountry.Name == "Utopia" && SelectedDivision < 4 ? 2 : 1);
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

            ChampionPromotionSpots = standings.Count(s => s.IsWinner);
            PromotionStandings = standings.Where(s => s.IsWinner || s.ConferenceRank <= 2)
                                        .OrderByDescending(s => s.IsWinner)
                                        .ThenBy(s => s.ConferenceRank)
                                        .ThenByDescending(s => s.Wins)
                                        .ThenByDescending(s => (s.PointDifference))
                                        .Select((s, idx) => new PromotionStanding(s)
                                        {
                                            PromotionRank = idx + 1,
                                            IsChampionPromotion = s.IsWinner,
                                            IsAutoPromotion = (!s.IsWinner && (idx + 1) <= (ChampionPromotionSpots + AutoPromotionSpots)),
                                            IsBotPromotion = (!s.IsWinner && (idx + 1) > (ChampionPromotionSpots + AutoPromotionSpots) && (idx + 1) <= (ChampionPromotionSpots + AutoPromotionSpots + BotPromotionSpots)),
                                            IsTotalPromotion = (!s.IsWinner && (idx + 1) > (ChampionPromotionSpots + AutoPromotionSpots + BotPromotionSpots) && (idx + 1) <= TotalPromotionSpots),
                                        }).ToList();
        }

        public bool CanSelectDivision => SelectedCountry != null;

        public async Task Activate()
        {
            Countries = (await _bbapi.GetCountries()).Where(c => c.Divisions > 1).ToList();
        }
    }
}
