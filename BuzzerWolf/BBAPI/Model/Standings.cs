using BuzzerWolf.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace BuzzerWolf.BBAPI.Model
{
    public class Standings
    {
        public Standings(XElement bbapiResponse)
        {
            League = new League(bbapiResponse.Descendants("league").First());
            Country = new Country(bbapiResponse.Descendants("country").First());

            Big8 = new List<TeamStanding>();
            Great8 = new List<TeamStanding>();

            var finalResults = bbapiResponse.Descendants("finalResults").FirstOrDefault();
            int? winningTeamId = null;
            List<int> relegatingTeams = new();
            if (finalResults != null)
            {
                IsFinal = true;
                winningTeamId = int.Parse(finalResults.Descendants("winner").First().Attribute("id")!.Value);
                foreach (var relegatingTeam in finalResults.Descendants("relegated"))
                {
                    relegatingTeams.Add(int.Parse(relegatingTeam.Attribute("id")!.Value));
                }
            }

            var conferences = bbapiResponse.Descendants("conference");
            foreach (var (teamStanding, index) in conferences.ElementAt(0).Descendants("team").WithIndex())
            {
                Big8.Add(new TeamStanding(teamStanding, index+1, League.Name, "Big 8", winningTeamId, relegatingTeams));
            }
            foreach (var (teamStanding, index) in conferences.ElementAt(1).Descendants("team").WithIndex())
            {
                Great8.Add(new TeamStanding(teamStanding, index+1, League.Name, "Great 8", winningTeamId, relegatingTeams));
            }
        }

        public League League { get; set; }
        public Country Country { get; set; }
        public List<TeamStanding> Big8 { get; }
        public List<TeamStanding> Great8 { get; }
        public bool IsFinal { get; set; } = false;
        public int? WinningTeamId { get; set; }
    }

    public class TeamStanding
    {
        public TeamStanding(XElement teamStanding, int conferenceRank, string leagueName, string conferenceName, int? winningTeamId, List<int> relegatingTeams) 
        {
            TeamId = int.Parse(teamStanding.Attribute("id")!.Value);
            TeamName = teamStanding.Descendants("teamName").First().Value;
            Wins = int.Parse(teamStanding.Descendants("wins").First().Value);
            Losses = int.Parse(teamStanding.Descendants("losses").First().Value);
            PointsFor = int.Parse(teamStanding.Descendants("pf").First().Value);
            PointsAgainst = int.Parse(teamStanding.Descendants("pa").First().Value);
            PointDifference = PointsFor - PointsAgainst;
            IsBot = teamStanding.Descendants("isBot").First().Value == "1";
            IsWinner = winningTeamId != null && winningTeamId == TeamId;
            IsRelegating = relegatingTeams.Any(t => t == TeamId);
            Forfeits = int.Parse(teamStanding.Descendants("forfeits").First().Value);
            ConferenceRank = conferenceRank;
            LeagueName = leagueName;
            ConferenceName = conferenceName;
        }

        public int TeamId { get; set; }
        public string TeamName { get; set; }
        public int Wins { get; set; }
        public int Losses { get; set; }
        public int PointsFor { get; set; }
        public int PointsAgainst { get; set; }
        public int PointDifference { get; set; }
        public bool IsBot { get; set; }
        public bool IsWinner { get; set; }
        public bool IsRelegating { get; set; }
        public int Forfeits { get; set; }
        public int ConferenceRank { get; set; }
        public string LeagueName { get; set; }
        public string ConferenceName { get; set; }
    }
}
