using BuzzerWolf.BBAPI;
using BuzzerWolf.Models;
using BuzzerWolf.ViewModels;
using BuzzerWolf.Views;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Windows;

namespace BuzzerWolf
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static IHost? Host { get; private set; }

        public App()
        {
            Host = Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) =>
                {
                    ConfigureServices(services);
                })
                .Build();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IBBAPIClient, BBAPIClient>();
            services.AddSingleton<BuzzerWolfContext>();

            services.AddSingleton<ProfileSelectionViewModel>();
            services.AddSingleton<ProfileSelection>();

            services.AddSingleton<AutoPromotionViewModel>();
            services.AddSingleton<AutoPromotion>();

            services.AddSingleton<SynchronizationViewModel>();

            services.AddSingleton<TeamHeadquartersViewModel>();

            services.AddSingleton<MainWindowViewModel>();
            services.AddSingleton<MainWindow>();
        }

        protected override async void OnStartup(StartupEventArgs startupArgs)
        {
            await Host!.StartAsync();

            var mainWindow = Host.Services.GetRequiredService<MainWindow>();
            mainWindow.Show();

            base.OnStartup(startupArgs);
        }

        protected override async void OnExit(ExitEventArgs exitArgs)
        {
            await Host!.StopAsync();
            base.OnExit(exitArgs);
        }
    }
}
