using BuzzerWolf.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Threading.Tasks;

namespace BuzzerWolf.ViewModels
{
    public partial class MainWindowViewModel : ObservableObject
    {
        public ProfileSelectionViewModel ProfileSelection { get; init; }
        private readonly AutoPromotionViewModel _autoPromotion;
        private readonly TeamHeadquartersViewModel _teamHeadquarters;
        public SynchronizationViewModel Synchronization { get; init; }

        public MainWindowViewModel(ProfileSelectionViewModel profileSelection, AutoPromotionViewModel autoPromotion, TeamHeadquartersViewModel teamHeadquarters, SynchronizationViewModel synchronization)
        {
            ProfileSelection = profileSelection;
            _autoPromotion = autoPromotion;
            _teamHeadquarters = teamHeadquarters;
            Synchronization = synchronization;
        }

        [ObservableProperty]
        private ObservableObject? activeViewModel;

        [RelayCommand]
        private void ShowAutoPromotion()
        {
            _autoPromotion.Activate();
            ActiveViewModel = _autoPromotion;
        }

        [RelayCommand]
        private async Task ShowTeamHeadquarters()
        {
            await _teamHeadquarters.Activate();
            ActiveViewModel = _teamHeadquarters;
            return;
        }

        [RelayCommand]
        private void ShowSynchronization()
        {
            var dialog = new SynchronizationDialog(Synchronization);

            _ = dialog.ShowDialog();
        }
    }
}
