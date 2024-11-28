using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BuzzerWolf.Models
{
    public class PlayoffSchedule
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int LeagueId { get; set; }
        public int Season { get; set; }
        public int MatchId { get; set; }
    }
}
