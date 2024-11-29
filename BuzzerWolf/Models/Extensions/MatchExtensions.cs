using System;
using System.Collections.Generic;
using System.Linq;

namespace BuzzerWolf.Models.Extensions
{
    public static class MatchExtensions
    {
        public static Match FromBBAPI(this BBAPI.Model.Match match)
        {
            return new Match
            {
                Id = match.Id,
                StartTime = match.StartTime,
                Type = FromBBAPI(match.Type),
                AwayTeamId = match.AwayTeam.TeamId,
                AwayTeamScore = match.AwayTeam.Score,
                HomeTeamId = match.HomeTeam.TeamId,
                HomeTeamScore = match.HomeTeam.Score,
                WinningTeamId = match.WinningTeamId,
            };
        }

        public static IEnumerable<Match> FromBBAPI(this IEnumerable<BBAPI.Model.Match> matches)
        {
            return matches.Select(FromBBAPI);
        }

        public static IEnumerable<Match> FromBBAPI(BBAPI.Model.Standings leagueStandings)
        {
            return leagueStandings.Playoffs.FromBBAPI();
        }

        private static MatchType FromBBAPI(string matchType) =>
            matchType switch
            {
                "league.rs" => MatchType.RegularSeason,
                "league.rs.tv" => MatchType.RegularSeason,
                "league.quarterfinal" => MatchType.QuarterFinal,
                "league.semifinal" => MatchType.SemiFinal,
                "league.final" => MatchType.Final,
                "league.relegation" => MatchType.Relegation,
                "cup" => MatchType.Cup,
                "bbb" => MatchType.BuzzerBeaterBest,
                "bbm" => MatchType.BuzzerBeaterMasters,
                "friendly" => MatchType.Scrimmage,
                "pl.rs" => MatchType.PLRegularSeason,
                _ => MatchType.Unknown
            };

        public static bool HasResult(this Match match) => match.WinningTeamId != null;
        public static string ResultForTeam(this Match match, int teamId) =>
            !match.TeamParticipatedInMatch(teamId) || !match.HasResult()
                ? string.Empty
                : $"{(match.WinningTeamId == teamId ? "W" : "L")} {(match.AwayTeamId == teamId ? $"{match.AwayTeamScore} - {match.HomeTeamScore}" : $"{match.HomeTeamScore} - {match.AwayTeamScore}")}";

        public static bool TeamPlayedOpponentInMatch(this Match match, int teamId, int opponentTeamId) => (match.AwayTeamId == teamId && match.HomeTeamId == opponentTeamId) || (match.HomeTeamId == teamId && match.AwayTeamId == opponentTeamId);
        public static int? OpponentTeamId(this Match match, int teamId) => !match.TeamParticipatedInMatch(teamId) ? null : (match.AwayTeamId == teamId ? match.HomeTeamId : match.AwayTeamId);
        public static bool TeamParticipatedInMatch(this Match match, int teamId) => match.AwayTeamId == teamId || match.HomeTeamId == teamId;
        public static bool TeamWonMatch(this Match match, int teamId) => match.WinningTeamId == teamId;
        public static bool TeamLostMatch(this Match match, int teamId) => match.TeamParticipatedInMatch(teamId) && match.WinningTeamId != teamId;
    }
}
