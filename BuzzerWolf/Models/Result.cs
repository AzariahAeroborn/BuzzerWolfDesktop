using Microsoft.EntityFrameworkCore;

namespace BuzzerWolf.Models
{
    [PrimaryKey(nameof(LeagueId), nameof(Season))]
    public class Result
    {
        public int LeagueId { get; set; }
        public int Season { get; set; }
        public int Winner { get; set; }
    }
}
