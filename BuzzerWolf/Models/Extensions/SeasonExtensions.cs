using System.Collections.Generic;
using System.Linq;

namespace BuzzerWolf.Models.Extensions
{
    public static class SeasonExtensions
    {
        public static Season FromBBAPI(this BBAPI.Model.Season season)
        {
            return new Season { Id = season.Id, Start = season.Start, Finish = season.Finish };
        }

        public static IEnumerable<Season> FromBBAPI(this IEnumerable<BBAPI.Model.Season> seasons)
        {
            return seasons.Select(FromBBAPI);
        }
    }
}
