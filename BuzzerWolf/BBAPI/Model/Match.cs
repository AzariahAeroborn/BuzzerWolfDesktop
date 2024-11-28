using System;
using System.Linq;
using System.Xml.Linq;

namespace BuzzerWolf.BBAPI.Model
{
    public class Match
    {
        public Match(XElement matchInfo) 
        {
            Id = int.Parse(matchInfo.Attribute("id")!.Value);
            StartTime = DateTime.Parse(matchInfo.Attribute("start")!.Value);
            Type = matchInfo.Attribute("type")!.Value;
            AwayTeam = new MatchTeamInfo(matchInfo.Descendants("awayTeam").First());
            HomeTeam = new MatchTeamInfo(matchInfo.Descendants("homeTeam").First());
            WinningTeamId = HomeTeam.Score == null ? null : (HomeTeam.Score > AwayTeam.Score ? HomeTeam.TeamId : AwayTeam.TeamId);
        }

        public int Id { get; set; }
        public DateTime StartTime { get; set; }
        public string Type { get; set; }
        public MatchTeamInfo AwayTeam { get; set; }
        public MatchTeamInfo HomeTeam { get; set; }
        public int? WinningTeamId { get; set; }
    }


    public class MatchTeamInfo
    {
        public MatchTeamInfo(XElement teamInfo)
        {
            TeamId = int.Parse(teamInfo.Attribute("id")!.Value);
            TeamName = teamInfo.Descendants("teamName").First().Value;
            var score = teamInfo.Descendants("score").FirstOrDefault();
            Score = (score != null) ? int.Parse(score.Value) : null;
        }

        public int TeamId { get; set; }
        public string TeamName { get; set; }
        public int? Score { get; set; }
    }
}
