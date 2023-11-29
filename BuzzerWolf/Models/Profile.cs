using System.ComponentModel.DataAnnotations;

namespace BuzzerWolf.Models
{
    public class Profile
    {
        [Key]
        public int TeamId { get; set; }
        public string User { get; set; }
        public string AccessKey { get; set; }
        public bool SecondTeam { get; set; }
        public string TeamName { get; set; }
    }
}
