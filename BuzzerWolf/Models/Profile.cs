using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BuzzerWolf.Models
{
    public class Profile
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int TeamId { get; set; }
        public string User { get; set; }
        public string AccessKey { get; set; }
        public bool SecondTeam { get; set; }
        public string TeamName { get; set; }
    }
}
