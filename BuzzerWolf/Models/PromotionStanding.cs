using BuzzerWolf.BBAPI.Model;

namespace BuzzerWolf.Models
{
    public class PromotionStanding
    {
        public PromotionStanding(TeamStanding teamStanding)
        {
            TeamId = teamStanding.TeamId;
            TeamName = teamStanding.TeamName;
            Wins = teamStanding.Wins;
            Losses = teamStanding.Losses;
            PointDifference = teamStanding.PointDifference;
            ConferenceRank = teamStanding.ConferenceRank;
            LeagueName = teamStanding.LeagueName;
            ConferenceName = teamStanding.ConferenceName;
        }

        public int TeamId { get; set; }
        public string TeamName { get; set; }
        public int Wins { get; set; }
        public int Losses { get; set; }
        public int PointDifference { get; set; }
        public int ConferenceRank { get; set; }
        public string LeagueName { get; set; }
        public string ConferenceName { get; set; }
        public int PromotionRank { get; set; }
        public bool IsChampionPromotion { get; set; }
        public bool IsAutoPromotion { get; set; }
        public bool IsBotPromotion { get; set; }
        public bool IsTotalPromotion { get; set; }
    }
}
