using BuzzerWolf.BBAPI;
using BuzzerWolf.Extensions;
using BuzzerWolf.Models;
using BuzzerWolf.Models.Extensions;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace BuzzerWolf.ViewModels
{
    public partial class SynchronizationViewModel : ObservableObject
    {
        private readonly IBBAPIClient _client;
        private readonly BuzzerWolfContext _context;
        private readonly ProfileSelectionViewModel _profile;

        public SynchronizationViewModel(IBBAPIClient client, BuzzerWolfContext context, ProfileSelectionViewModel profile)
        {
            _client = client;
            _context = context;
            _profile = profile;
            _profile.PropertyChanged += new PropertyChangedEventHandler(OnProfileSelectionChanged);
        }

        private void OnProfileSelectionChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SelectedProfile")
            {
                PopulateSyncList();
            }
            if (e.PropertyName == "IsLoggedIn")
            {
                if (_profile.IsLoggedIn)
                {
                    Task.Run(() => Sync(false));
                }
            }
        }

        private void PopulateSyncList()
        {
            SynchronizedTables.ReplaceRange(_context.Sync);
        }

        [ObservableProperty]
        private ObservableRangeCollection<Sync> synchronizedTables = new();
        [ObservableProperty]
        private string statusDetail = string.Empty;
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(StatusSummary))]
        [NotifyCanExecuteChangedFor(nameof(ForceSyncCommand))]
        private bool running = false;

        public string StatusSummary => Running ? "Synchronization running" : "Synchronization up-to-date";

        [RelayCommand(CanExecute = nameof(CanForceSync))]
        private async Task ForceSync()
        {
            await Sync(true);
        }

        private async Task Sync(bool force) { 
            Running = true;
            StatusDetail = "Synchronization starting...";
            if (force || SynchronizedTables.Any(t => t.DataTable == "seasons" && t.NextAutoSync < DateTimeOffset.UtcNow)) await SyncSeasons();
            await FinalizeSync();
            StatusDetail = "Synchronization complete";
            Running = false;
        }

        private bool CanForceSync() => !Running;

        private async Task FinalizeSync()
        {
            await _context.SaveChangesAsync();
            PopulateSyncList();
        }

        private readonly int seasonLength = 14 * 7;
        private async Task SyncSeasons()
        {
            StatusDetail = "Synchronizing season list...";
            var seasonList = await _client.GetSeasons();
            _context.Seasons.UpsertRange(SeasonExtensions.FromBBAPI(seasonList.Seasons)).On(s => s.Id).Run();
            
            var syncRecord = _context.Sync.Where(s => s.DataTable == "seasons").FirstOrDefault();
            if (syncRecord != null)
            {
                syncRecord.LastSync = DateTimeOffset.UtcNow;
                syncRecord.NextAutoSync = seasonList.Seasons.Last().Start.AddDays(seasonLength);
            }
        }
    }
}
