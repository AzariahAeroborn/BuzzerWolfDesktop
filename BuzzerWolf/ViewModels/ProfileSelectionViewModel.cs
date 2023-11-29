using BuzzerWolf.BBAPI;
using BuzzerWolf.Models;
using BuzzerWolf.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BuzzerWolf.ViewModels
{
    public partial class ProfileSelectionViewModel : ObservableObject
    {
        private readonly IBBAPIClient _client;
        private readonly BuzzerWolfContext _context;

        public ProfileSelectionViewModel(IBBAPIClient client, BuzzerWolfContext context)
        {
            _client = client;
            _context = context;
            LoadProfiles();
        }

        private void LoadProfiles()
        {
            AvailableProfiles = _context.Profiles.ToList();
        }

        [ObservableProperty]
        public List<Profile> availableProfiles = new();
        [ObservableProperty]
        public Profile? selectedProfile;

        [ObservableProperty]
        private string status = string.Empty;
        [ObservableProperty]
        private bool isLoggedIn = false;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(RemoveProfileCommand))]
        private bool canRemoveProfile;

        partial void OnSelectedProfileChanged(Profile? value)
        {
            Task.Run(LoginBBAPI);
            CanRemoveProfile = SelectedProfile != null;
        }

        private async Task LoginBBAPI()
        {
            if (SelectedProfile == null) { return; }
            
            Status = "Connecting to BBAPI";
            try
            {
                if (await _client.Login(SelectedProfile.User, SelectedProfile.AccessKey, SelectedProfile.SecondTeam))
                {
                    Status = $"Logged in as {SelectedProfile.TeamName}";
                    IsLoggedIn = true;
                };
            }
            catch
            {
                Status = "Unhandled exception while connecting to BBAPI";
                IsLoggedIn = false;
            }
        }

        [RelayCommand]
        public void AddProfile()
        {
            var login = new LoginDialogViewModel(_client, _context);
            var dialog = new LoginDialog(login);

            bool dialogResult = (bool)dialog.ShowDialog()!;
            if (dialogResult)
            {
                LoadProfiles();
                SelectedProfile = AvailableProfiles.Where(p => p.TeamId == login.LoggedInTeam?.TeamId).First();
            }
        }

        [RelayCommand(CanExecute = nameof(CanRemoveProfile))]
        public void RemoveProfile()
        {
            if (SelectedProfile == null) { return; }
            
            _client.Logout();
            Status = string.Empty;
            _context.Profiles.Remove(SelectedProfile);
            SelectedProfile = null;
            _context.SaveChanges();
            LoadProfiles();
        }
    }
}
