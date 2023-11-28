using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace BuzzerWolf.BBAPI.Model
{
    public class Schedule
    {
        public Schedule(XElement bbapiResponse)
        {
            TeamId = int.Parse(bbapiResponse.Descendants("schedule").First().Attribute("teamid")!.Value);
            Matches = new List<Match>();
            foreach (var matchInfo in bbapiResponse.Descendants("match"))
            {
                Matches.Add(new Match(matchInfo));
            }

        }

        public int TeamId { get; set; }
        public int Season { get; set; }
        public List<Match> Matches { get; }
    }
}
