using BuzzerWolf.BBAPI.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BuzzerWolf.BBAPI
{
    public interface IBBAPIClient
    {
        Task<bool> Login(string userName, string accessKey, bool secondTeam);
        Task<TeamInfo> VerifyLogin(string userName, string accessKey, bool secondTeam);
        Task<bool> Logout();
        Task<Arena> GetArena(int? teamId = null);
        void GetBoxScore(int matchId);
        void GetEconomy();
        Task<Roster> GetRoster(int? teamId = null);
        Task<Player> GetPlayer(int playerId);
        Task<Schedule> GetSchedule (int? teamId = null, int? season = null);
        Task<SeasonList> GetSeasons();
        Task<Standings> GetStandings(int? leagueId = null, int? season = null);
        Task<TeamInfo> GetTeamInfo(int? teamId = null);
        void GetTeamStats(int? teamId = null, int? season = null, string mode = "averages");
        Task<List<Country>> GetCountries();
        Task<List<League>> GetLeagues(int countryId, int level);
    }
}