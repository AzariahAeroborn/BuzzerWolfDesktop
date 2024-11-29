using System.Collections.Generic;
using System.Linq;

namespace BuzzerWolf.Models.Extensions
{
    public static class StandingsExtensions
    {
        public static IEnumerable<Standings> FromBBAPI(this BBAPI.Model.Standings standings)
        {
            return standings.Great8.Select(ts => ts.FromBBAPI(standings, Conference.Great8, ts.ConferenceRank))
                .Concat(standings.Big8.Select(ts => ts.FromBBAPI(standings, Conference.Big8, ts.ConferenceRank)));
        }

        private static Standings FromBBAPI(this BBAPI.Model.TeamStanding teamStanding, BBAPI.Model.Standings standings, Conference conference, int conferenceRank)
        {
            return new Standings
            {
                LeagueId = standings.League.Id,
                Season = standings.Season,
                Conference = conference,
                ConferenceRank = conferenceRank,
                TeamId = teamStanding.TeamId,
                TeamName = teamStanding.TeamName,
                Wins = teamStanding.Wins,
                Losses = teamStanding.Losses,
                PointsFor = teamStanding.PointsFor,
                PointsAgainst = teamStanding.PointsAgainst,
                IsBot = teamStanding.IsBot,
            };
        }

        public static IOrderedEnumerable<Standings> OrderedConferenceStandings(this IEnumerable<Standings> standings, Conference conference)
        {
            return standings.Where(s => s.Conference == conference)
                            .OrderByDescending(s => s.Wins)
                            .ThenByDescending(s => s.PointsFor - s.PointsAgainst);
        }
    }
}
