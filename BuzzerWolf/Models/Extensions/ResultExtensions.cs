namespace BuzzerWolf.Models.Extensions
{
    public static class ResultExtensions
    {
        public static Result? FromBBAPI(BBAPI.Model.Standings standings)
        {
            if (!standings.IsFinal)
                return null;

            return new Result
            {
                LeagueId = standings.League.Id,
                Season = standings.Season,
                Winner = (int)standings.WinningTeamId!,
            };
        }
    }
}
