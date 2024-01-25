using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NectarRCON.Interfaces;
using NectarRCON.Rcon;
using NectarRCON.Services;
using NectarRCON.ViewModels;
using NectarRCON.Windows;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using Microsoft.Extensions.Logging;
using NectarRCON.Dp;
using Serilog;
using Wpf.Ui.Mvvm.Contracts;
using Wpf.Ui.Mvvm.Services;

namespace NectarRCON;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App
{
    private static readonly ILoggerFactory LoggerFactory = new LoggerFactory();
    private static string LogFileName = $"logs/program/log{DateTime.Now:yyyyMMddhhmm}.log";

    private static readonly IHost Host = Microsoft.Extensions.Hosting.Host
        .CreateDefaultBuilder()
        .ConfigureLogging(builder =>
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.File(LogFileName,
                    rollingInterval: RollingInterval.Infinite, flushToDiskInterval: TimeSpan.FromSeconds(1))
                .CreateLogger();

            builder.AddSerilog();
        })
        .ConfigureServices((context, services) =>
        {
            services.AddSingleton(LoggerFactory);
            services.AddSingleton(typeof(ILogger<>), typeof(Logger<>));

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
        return (Host.Services.GetService(typeof(T)) as T)!;
    }

    public static T GetService<T>(Type type)
        where T : class
    {
        return Host.Services.GetServices<T>().FirstOrDefault(t => t.GetType() == type)!;
    }

    private async void OnStartup(object sender, StartupEventArgs e)
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

        foreach (var rconEncoding in Enum.GetValues<Dp.RconEncoding>())
        {
            rconEncoding.GetEncoding();
        }

        AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        await Host.StartAsync();
    }

    private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        var exception = e.ExceptionObject as Exception;
        Log.Error("未经处理的异常: {0}", exception);

        MessageBox.Show(
            exception +
            $"\n\n程序遇到异常,即将退出!\n建议前往Github提交Issue\n请前往日志查看详细信息!\nCheck log: {Path.Combine(Environment.CurrentDirectory, LogFileName).Replace("\\", "/")}",
            "程序崩溃", MessageBoxButton.OK, MessageBoxImage.Error);
        Environment.Exit(1);
    }

    private async void OnExit(object sender, ExitEventArgs e)
    {
        await Host.StopAsync();
        Host.Dispose();
    }
}