using System.Collections.Generic;
using System.Linq;

namespace BuzzerWolf.Models.Extensions
{
    public static class LeagueExtensions
    {
        public static IEnumerable<League> FromBBAPI(this IEnumerable<BBAPI.Model.League> leagues, int countryId, int divisionLevel)
        {
            return leagues.Select(l => l.FromBBAPI(countryId, divisionLevel));
        }

        public static League FromBBAPI(this BBAPI.Model.League league, int countryId, int divisionLevel)
        {
            return new League
            {
                Id = league.Id,
                Name = league.Name,
                CountryId = countryId,
                DivisionLevel = divisionLevel
            };
        }
    }
}
