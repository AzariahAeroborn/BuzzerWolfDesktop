using System.Linq;
using System.Xml.Linq;

namespace BuzzerWolf.BBAPI.Model
{
    public class Arena
    {
        public Arena(XElement bbapiResponse)
        {
            TeamId = int.Parse(bbapiResponse.Descendants("arena").First().Attribute("teamid")!.Value);
            Name = bbapiResponse.Descendants("name").First().Value;

            var bleachers = bbapiResponse.Descendants("bleachers").First();
            Bleachers = int.Parse(bleachers.Value);
            BleachersPrice = int.Parse(bleachers.Attribute("price")!.Value);

            var lowerTier = bbapiResponse.Descendants("lowerTier").First();
            LowerTier = int.Parse(lowerTier.Value);
            LowerTierPrice = int.Parse(lowerTier.Attribute("price")!.Value);

            var courtside = bbapiResponse.Descendants("courtside").First();
            Courtside = int.Parse(courtside.Value);
            CourtsidePrice = int.Parse(courtside.Attribute("price")!.Value);

            var luxury = bbapiResponse.Descendants("luxury").First();
            Luxury = int.Parse(luxury.Value);
            LuxuryPrice = int.Parse(luxury.Attribute("price")!.Value);
        }

        public int TeamId { get; set; }
        public string Name { get; set; }
        public int Bleachers { get; set; }
        public int BleachersPrice { get; set; }
        public int LowerTier { get; set; }
        public int LowerTierPrice { get; set; }
        public int Courtside { get; set; }
        public int CourtsidePrice { get; set; }
        public int Luxury { get; set; }
        public int LuxuryPrice { get; set; }
    }
}
