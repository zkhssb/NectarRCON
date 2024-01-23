using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NectarRCON.Interfaces;
using NectarRCON.Rcon;
using NectarRCON.Services;
using NectarRCON.ViewModels;
using NectarRCON.Windows;
using System;
using System.Linq;
using System.Text;
using System.Windows;
using NectarRCON.Dp;
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

            services.AddSingleton<IConnectingDialogService, ConnectingDialogService>();

            // Rcon Connections
            services.AddSingleton<IRconConnectionInfoService, RconConnectionInfoService>();
            services.AddSingleton<IRconConnection, RconSingleConnection>();
            services.AddSingleton<IRconConnection, RconMultiConnection>();

            services.AddSingleton<IMessageBoxService, MessageBoxService>();

            services.AddSingleton<IGroupService, GroupService>();


            services.AddScoped<INavigationWindow, MainWindow>();
            services.AddScoped<MainWindowViewModel>();

            services.AddTransient<AddServerWindow>();
            services.AddTransient<AddServerWindowViewModel>();

        }).Build();
    public static T GetService<T>()
        where T : class
    {
        return (_host.Services.GetService(typeof(T)) as T)!;
    }

    public static T GetService<T>(Type type)
    where T : class
    {
        return (_host.Services.GetServices<T>().Where(t => t.GetType() == type).FirstOrDefault())!;
    }

    private async void OnStartup(object sender, StartupEventArgs e)
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

        foreach (var rconEncoding in Enum.GetValues<Dp.RconEncoding>())
        {
            rconEncoding.GetEncoding();
        }
        
        await _host.StartAsync();
    }

    private async void OnExit(object sender, ExitEventArgs e)
    {
        await _host.StopAsync();
        _host.Dispose();
    }
}
