using Microsoft.Extensions.Hosting;
using NectarRCON.Core.Helper;
using NectarRCON.Interfaces;
using NectarRCON.Models;
using System;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Extensions.Logging;
using Serilog;
using Wpf.Ui.Mvvm.Contracts;

namespace NectarRCON.Services;
public class ApplicationHostService(
    IServiceProvider serviceProvider,
    INavigationService navigationService,
    IThemeService themeService,
    ILanguageService languageService,
    IConfigService configService)
    : IHostedService
{
    private readonly INavigationService _navigationService = navigationService;
    private INavigationWindow? _navigationWindow;

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        Log.Information("Starting NectarRCON...");
        await HandleActivationAsync();
    }
    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
    }
    private void LoadConfig()
    {
        switch (configService.GetConfig().Theme)
        {
            case ETheme.System:
                themeService.SetTheme(Win32Helper.GetWindowsTheme() ? Wpf.Ui.Appearance.ThemeType.Dark : Wpf.Ui.Appearance.ThemeType.Light);
                break;
            case ETheme.Dark:
                themeService.SetTheme(Wpf.Ui.Appearance.ThemeType.Dark);
                break;
            case ETheme.Light:
                themeService.SetTheme(Wpf.Ui.Appearance.ThemeType.Light);
                break;
            default:
                break;
        }
        if (configService.GetConfig().LanguageName == string.Empty)
        {
            languageService.SelectLanguage();
            configService.GetConfig().LanguageName = languageService.GetKey("file.language");
            configService.Save();
        }
        else
        {
            languageService.SelectLanguage(configService.GetConfig().LanguageName, false);
        }
    }
    private async Task HandleActivationAsync()
    {
        await Task.CompletedTask;

        if (!Application.Current.Windows.OfType<Container>().Any())
        {
            _navigationWindow = (serviceProvider.GetService(typeof(INavigationWindow)) as INavigationWindow)!;
            Log.Information("Show MainWindow...");
            _navigationWindow!.ShowWindow();
        }

        LoadConfig();

        await Task.CompletedTask;
    }
}
