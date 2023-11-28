using System.Xml.Linq;

namespace BuzzerWolf.BBAPI.Model
{
    public class Country
    {
        public Country(XElement countryInfo)
        {
            Id = int.Parse(countryInfo.Attribute("id")!.Value);
            Name = countryInfo.Value;
            Divisions = int.TryParse(countryInfo.Attribute("divisions")?.Value, out var divisions) ? divisions : null;
            FirstSeason = int.TryParse(countryInfo.Attribute("firstSeason")?.Value, out var firstSeason) ? firstSeason : null;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public int? Divisions { get; set; }
        public int? FirstSeason { get; set; }
    }
}