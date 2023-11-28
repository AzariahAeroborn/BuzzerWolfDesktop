using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace BuzzerWolf.BBAPI.Model
{
    public class Roster
    {
        public Roster(XElement bbapiResponse) 
        {
            TeamId = int.Parse(bbapiResponse.Descendants("roster").First().Attribute("teamid")!.Value);
            Players = new List<Player>();
            foreach (var playerInfo in bbapiResponse.Descendants("player"))
            {
                Players.Add(new Player(playerInfo));
            }
        }

        public int TeamId { get; set; }
        public List<Player> Players { get; }
    }
}
