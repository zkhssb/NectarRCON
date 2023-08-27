using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NectarRCON.Interfaces;
using NectarRCON.Rcon;
using NectarRCON.Services;
using NectarRCON.ViewModels;
using NectarRCON.Views.Pages;
using NectarRCON.Windows;
using System.Windows;
using Wpf.Ui.Mvvm.Contracts;
using Wpf.Ui.Mvvm.Services;

namespace NectarRCON;
/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App
{
    private static readonly IHost _host = Host
        .CreateDefaultBuilder()
        .ConfigureServices((context, services) =>
        {
            services.AddHostedService<ApplicationHostService>();

            services.AddSingleton<ILanguageService, LanguageService>();
            services.AddSingleton<IConfigService, ConfigService>();
            services.AddSingleton<ILogService, LogService>();
            services.AddSingleton<IServerPasswordService, ServerPasswordService>();

            services.AddSingleton<IThemeService, ThemeService>();
            services.AddSingleton<INavigationService, NavigationService>();
            services.AddSingleton<ISnackbarService, SnackbarService>();
            services.AddSingleton<IServerInformationService, ServerInformationService>();
            services.AddSingleton<IRconConnection, RconSingleConnection>();

            services.AddSingleton<IConnectingDialogService, ConnectingDialogService>();

            // Rcon Connections
            services.AddSingleton<IRconConnectionInfoService, RconConnectionInfoService>();
            services.AddSingleton<IRconConnection, RconSingleConnection>();
            //services.AddSingleton<IRconConnection, RconMultiConnection>();


            services.AddScoped<INavigationWindow, MainWindow>();
            services.AddScoped<MainWindowViewModel>();

            services.AddScoped<ServersPage>();
            services.AddScoped<ServersPageViewModel>();

            services.AddTransient<AddServerWindow>();
            services.AddTransient<AddServerWindowViewModel>();

        }).Build();
    public static T GetService<T>()
        where T : class
    {
        return (_host.Services.GetService(typeof(T)) as T)!;
    }

    private async void OnStartup(object sender, StartupEventArgs e)
    {
        await _host.StartAsync();
    }

    private async void OnExit(object sender, ExitEventArgs e)
    {
        await _host.StopAsync();
        _host.Dispose();
    }
}
