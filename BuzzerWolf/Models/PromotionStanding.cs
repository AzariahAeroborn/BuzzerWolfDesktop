namespace BuzzerWolf.Models
{
    public class PromotionStanding
    {
        public PromotionStanding(Standings standings, string leagueName)
        {
            TeamId = standings.TeamId;
            TeamName = standings.TeamName;
            Wins = standings.Wins;
            Losses = standings.Losses;
            PointDifference = standings.PointsFor - standings.PointsAgainst;
            ConferenceRank = standings.ConferenceRank;
            LeagueName = leagueName;
            ConferenceName = standings.Conference.ToString();
        }

        public int TeamId { get; set; }
        public string TeamName { get; set; }
        public int Wins { get; set; }
        public int Losses { get; set; }
        public int PointDifference { get; set; }
        public int ConferenceRank { get; set; }
        public string LeagueName { get; set; }
        public string ConferenceName { get; set; }
        public string NextOpponent { get; set; }
        public string NextOpponentLastResult { get; set; }
        public string RemainingStrengthOfSchedule { get; set; }
        public int PromotionRank { get; set; }
        public bool IsChampionPromotion { get; set; }
        public bool IsEliminated { get; set; }
        public bool IsAutoPromotion { get; set; }
        public bool IsBotPromotion { get; set; }
        public bool IsTotalPromotion { get; set; }
    }
}
