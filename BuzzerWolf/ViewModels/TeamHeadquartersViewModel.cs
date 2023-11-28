using BuzzerWolf.BBAPI;
using BuzzerWolf.BBAPI.Model;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuzzerWolf.ViewModels
{
    public partial class TeamHeadquartersViewModel : ObservableObject
    {
        private readonly IBBAPIClient _bbapi;
        public TeamHeadquartersViewModel(IBBAPIClient bbapi)
        {
            _bbapi = bbapi;
        }

        [ObservableProperty]
        private TeamInfo? teamInfo;

        public async Task Activate()
        {
            TeamInfo = await _bbapi.GetTeamInfo();
        }
    }
}
