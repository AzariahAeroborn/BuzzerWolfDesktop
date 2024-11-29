using System.Collections.Generic;
using System.Linq;

namespace BuzzerWolf.Models.Extensions
{
    public static class CountryExtensions
    {
        public static Country FromBBAPI(this BBAPI.Model.Country country)
        {
            return new Country { Id = country.Id, Name = country.Name, Divisions = country.Divisions, FirstSeason = country.FirstSeason };
        }

        public static IEnumerable<Country> FromBBAPI (this IEnumerable<BBAPI.Model.Country> countries)
        {
            return countries.Select(FromBBAPI);
        }
    }
}
