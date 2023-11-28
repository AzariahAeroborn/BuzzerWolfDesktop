using System.Xml.Linq;

namespace BuzzerWolf.BBAPI.Model
{
    public class League
    {
        public League(XElement leagueInfo)
        {
            Id = int.Parse(leagueInfo.Attribute("id")!.Value);
            Name = leagueInfo.Value;
        }

        public int Id { get; set; }
        public string Name { get; set; }
    }
}
