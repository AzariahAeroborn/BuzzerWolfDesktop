using BuzzerWolf.BBAPI;
using BuzzerWolf.BBAPI.Exceptions;
using BuzzerWolf.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace BuzzerWolf.ViewModels
{
    public partial class LoginDialogViewModel : ObservableObject
    {
        private readonly IBBAPIClient _client;
        private readonly BuzzerWolfContext _context;
        public LoginDialogViewModel(IBBAPIClient client, BuzzerWolfContext context)
        {
            _client = client;
            _context = context;
        }

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(LoginBBAPICommand))]
        public string user = string.Empty;
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(LoginBBAPICommand))]
        public string accessKey = string.Empty;
        [ObservableProperty]
        public bool secondTeam = false;
        [ObservableProperty]
        private string status = string.Empty;
        [ObservableProperty]
        private Profile? loggedInTeam;

        [RelayCommand(CanExecute = nameof(CanLoginBBAPI))]
        private async Task LoginBBAPI(Window loginDialog)
        {
            Status = "Connecting to BBAPI";
            try
            {
                var teamInfo = await _client.VerifyLogin(User, AccessKey, SecondTeam);

                if (_context.Profiles.Any(p => p.TeamId == teamInfo.Id))
                {
                    Status = $"A profile already exists for {teamInfo.TeamName}";
                }
                else
                {
                    Status = $"Logged in as {teamInfo.TeamName}";
                    LoggedInTeam = new Profile
                    {
                        AccessKey = AccessKey,
                        User = User,
                        SecondTeam = SecondTeam,
                        TeamId = teamInfo.Id,
                        TeamName = teamInfo.TeamName,
                    };

                    _context.Profiles.Add(LoggedInTeam);
                    _context.Sync.AddRange(Sync.InitializeFor(LoggedInTeam.TeamId));
                    _context.SaveChanges();

                    loginDialog.DialogResult = true;
                    loginDialog.Close();
                }
            }
            catch (UnauthorizedException)
            {
                Status = "Invalid user or access key";
            }
            catch (BBAPIServerErrorException)
            {
                Status = "BBAPI Server Error - please wait and try again";
            }
            catch
            {
                Status = "Unhandled exception while connecting to BBAPI";
            }
        }

        private bool CanLoginBBAPI() => !string.IsNullOrEmpty(User) && !string.IsNullOrEmpty(AccessKey);
    }
}
