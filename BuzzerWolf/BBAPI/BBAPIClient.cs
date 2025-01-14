﻿using BuzzerWolf.BBAPI.Exceptions;
using BuzzerWolf.BBAPI.Model;
using Microsoft.AspNetCore.WebUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace BuzzerWolf.BBAPI
{
    public class BBAPIClient : IBBAPIClient
    {
        private readonly SocketsHttpHandler _handler;
        private readonly HttpClient _client;

        public BBAPIClient()
        {
            _handler = new SocketsHttpHandler { PooledConnectionLifetime = TimeSpan.FromMinutes(15) };
            _client = CreateBBAPIClient();
        }

        private HttpClient CreateBBAPIClient()
        {
            return new HttpClient(_handler)
            {
                BaseAddress = new Uri("https://bbapi.buzzerbeater.com")
            };
        }

        public async Task<bool> Login(string userName, string accessKey, bool secondTeam)
        {
            return await Login(userName, accessKey, secondTeam, _client);
        }

        private async Task<bool> Login(string userName, string accessKey, bool secondTeam, HttpClient client)
        {
            var bbapi = await CallAPI("login.aspx", new Dictionary<string, string?>()
            {
                { "login", userName },
                { "code", accessKey },
                { "secondTeam", secondTeam ? "1" : null }
            });

            if (bbapi.IsSuccess)
            {
                return true;
            }
            else
            {
                switch (bbapi.Error)
                {
                    case "NotAuthorized":
                        throw new UnauthorizedException();
                    case "ServerError":
                        throw new BBAPIServerErrorException();
                    default:
                        throw new UnexpectedResponseException();
                }
            }
        }

        public async Task<TeamInfo> VerifyLogin(string userName, string accessKey, bool secondTeam)
        {
            var client = CreateBBAPIClient();
            if (await Login(userName, accessKey, secondTeam, client))
            {
                return await GetTeamInfo();
            }

            throw new UnexpectedResponseException();
        }

        public async Task<bool> Logout()
        {
            var bbapi = await CallAPI("logout.aspx", new Dictionary<string, string?>());

            return bbapi.IsSuccess;
        }

        public async Task<Arena> GetArena(int? teamId = null)
        {
            var bbapi = await CallAPI("arena.aspx", new Dictionary<string, string?>()
            {
                { "teamId", teamId?.ToString() }
            });

            if (bbapi.IsSuccess)
            {
                return new Arena(bbapi.Response);
            }
            else
            {
                throw new UnexpectedResponseException();
            }
        }

        public void GetBoxScore(int matchId)
        {
            throw new NotImplementedException();
        }

        public void GetEconomy()
        {
            throw new NotImplementedException();
        }

        public async Task<Roster> GetRoster(int? teamId = null)
        {
            var bbapi = await CallAPI("roster.aspx", new Dictionary<string, string?>()
            {
                { "teamid", teamId?.ToString() }
            });

            if (bbapi.IsSuccess)
            {
                return new Roster(bbapi.Response);
            }
            else
            {
                throw new UnexpectedResponseException();
            }
        }

        public async Task<Player> GetPlayer(int playerId)
        {
            var bbapi = await CallAPI("player.aspx", new Dictionary<string, string?>()
            {
                { "playerid", playerId.ToString() }
            });

            if (bbapi.IsSuccess)
            {
                return new Player(bbapi.Response);
            }
            else
            {
                throw new UnexpectedResponseException();
            }
        }

        public async Task<Schedule> GetSchedule(int? teamId = null, int? season = null)
        {
            var bbapi = await CallAPI("schedule.aspx", new Dictionary<string, string?>()
            {
                { "teamid", teamId?.ToString() },
                { "season", season?.ToString() }
            });

            if (bbapi.IsSuccess)
            {
                return new Schedule(bbapi.Response);
            }
            else
            {
                throw new UnexpectedResponseException();
            }
        }

        public async Task<SeasonList> GetSeasons()
        {
            var bbapi = await CallAPI("seasons.aspx", new Dictionary<string, string?>());

            if (bbapi.IsSuccess)
            {
                return new SeasonList(bbapi.Response);
            }
            else
            {
                throw new UnexpectedResponseException();
            }
        }

        public async Task<Standings> GetStandings(int? leagueId = null, int? season = null)
        {
            var bbapi = await CallAPI("standings.aspx", new Dictionary<string, string?>()
            {
                { "leagueid", leagueId?.ToString() },
                { "season", season?.ToString() }
            });

            if (bbapi.IsSuccess)
            {
                return new Standings(bbapi.Response);
            }
            else
            {
                throw new UnexpectedResponseException();
            }
        }

        public async Task<TeamInfo> GetTeamInfo(int? teamId = null)
        {
            var bbapi = await CallAPI("teaminfo.aspx", new Dictionary<string, string?>()
            {
                { "teamid", teamId?.ToString() }
            });

            if (bbapi.IsSuccess)
            {
                return new TeamInfo(bbapi.Response);
            }
            else
            {
                throw new UnexpectedResponseException();
            }
        }

        public void GetTeamStats(int? teamId = null, int? season = null, string mode = "averages")
        {
            throw new NotImplementedException();
        }

        public async Task<List<Country>> GetCountries()
        {
            var bbapi = await CallAPI("countries.aspx", new Dictionary<string, string?>());

            if (bbapi.IsSuccess)
            {
                var countryList = new List<Country>();
                foreach (var country in bbapi.Response.Descendants("country"))
                {
                    countryList.Add(new Country(country));
                }
                return countryList;
            }
            else
            {
                throw new UnexpectedResponseException();
            }
        }

        public async Task<List<League>> GetLeagues(int countryId, int level)
        {
            var bbapi = await CallAPI("leagues.aspx", new Dictionary<string, string?>()
            {
                { "countryid", countryId.ToString() },
                { "level", level.ToString() }
            });

            if (bbapi.IsSuccess)
            {
                var leaguesList = new List<League>();
                foreach (var league in bbapi.Response.Descendants("league"))
                {
                    leaguesList.Add(new League(league));
                }
                return leaguesList;
            }
            else
            {
                throw new UnexpectedResponseException();
            }
        }

        private SemaphoreSlim apiThrottler = new(50);
        private async Task<BBAPIResponse> CallAPI(string requestPath, Dictionary<string, string?> queryParams)
        {
            try
            {
                await apiThrottler.WaitAsync();
                var response = await _client.GetAsync(QueryHelpers.AddQueryString(requestPath, queryParams.Where(q => q.Value != null)));
                XElement bbapiResponse = XElement.Load(await response.Content.ReadAsStreamAsync());

                var error = bbapiResponse.Descendants("error").FirstOrDefault();
                if (error != null)
                {
                    var errorType = error.Attribute("message")!.Value;
                    switch (errorType)
                    {
                        case "NotAuthorized":
                            throw new UnauthorizedException();
                        case "ServerError":
                            throw new BBAPIServerErrorException();
                        case "UnknownTeamID":
                            throw new UnexpectedResponseException("Invalid team ID provided");
                        case "UnknownPlayerID":
                            throw new UnexpectedResponseException("Invalid player ID provided");
                        case "UnknownLeagueID":
                            throw new UnexpectedResponseException("Invalid league ID provided");
                        case "UnknownSeason":
                            throw new UnexpectedResponseException("Invalid season provided");
                        default:
                            return new BBAPIResponse()
                            {
                                IsSuccess = false,
                                Error = errorType,
                                Response = bbapiResponse
                            };
                    }
                }

                return new BBAPIResponse()
                {
                    IsSuccess = true,
                    Error = null,
                    Response = bbapiResponse
                };
            }
            finally
            {
                apiThrottler.Release();
            }
        }
    }
}
