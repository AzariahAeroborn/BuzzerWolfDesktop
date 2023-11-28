using BuzzerWolf.BBAPI;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Threading.Tasks;

namespace BuzzerWolf.ViewModels
{
    public partial class LoginViewModel : ObservableObject
    {
        private readonly IBBAPIClient _client;
        public LoginViewModel(IBBAPIClient client)
        {
            _client = client;
        }

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(LoginBBAPICommand))]
        public string user = string.Empty;
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(LoginBBAPICommand))]
        public string accessKey = string.Empty;
        [ObservableProperty]
        private string status = string.Empty;
        [ObservableProperty]
        private bool isLoggedIn = false;

        [RelayCommand(CanExecute = nameof(CanLoginBBAPI))]
        private async Task LoginBBAPI()
        {
            Status = "Connecting to BBAPI";
            try
            {
                if (await _client.Login(User, AccessKey))
                {
                    Status = $"Logged in as {(await _client.GetTeamInfo()).TeamName}";
                    IsLoggedIn = true;
                };
            }
            catch
            {
                Status = "Unhandled exception while connecting to BBAPI";
                IsLoggedIn = false;
            }
        }

        private bool CanLoginBBAPI()
        {
            return !string.IsNullOrEmpty(User) && !string.IsNullOrEmpty(AccessKey);
        }
    }
}
