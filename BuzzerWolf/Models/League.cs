using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BuzzerWolf.Models
{
    public class League
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        public int CountryId { get; set; }
        public string Name { get; set; }
        public int DivisionLevel { get; set; }

        public Country Country { get; set; }
        public ICollection<Standings> Standings { get; set; }
        public ICollection<Result> Results { get; set; }
        public ICollection<PlayoffSchedule> PlayoffSchedules { get; set; }
    }
}
