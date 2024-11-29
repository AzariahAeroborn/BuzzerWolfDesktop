using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BuzzerWolf.Models
{
    public class Match
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        public DateTime StartTime { get; set; }
        public MatchType Type { get; set; }
        public int AwayTeamId { get; set; }
        public int? AwayTeamScore { get; set; }
        public int HomeTeamId { get; set; }
        public int? HomeTeamScore { get; set; }
        public int? WinningTeamId { get; set; }
    }
}
