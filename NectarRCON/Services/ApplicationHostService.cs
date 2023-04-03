using Microsoft.Extensions.Hosting;
using NectarRCON.Helper;
using NectarRCON.Interfaces;
using NectarRCON.Models;
using System;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Wpf.Ui.Mvvm.Contracts;

namespace NectarRCON.Services;
public class ApplicationHostService : IHostedService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly INavigationService _navigationService;
    private readonly IThemeService _themeService;
    private readonly IConfigService _configService;
    private readonly ILanguageService _languageService;
    private INavigationWindow? _navigationWindow;
    public ApplicationHostService(IServiceProvider serviceProvider, INavigationService navigationService, IThemeService themeService, ILanguageService languageService, IConfigService configService)
    {
        _serviceProvider = serviceProvider;
        _navigationService = navigationService;
        _themeService = themeService;
        _languageService = languageService;
        _configService = configService;
    }
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await HandleActivationAsync();
    }
    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
    }
    private void LoadConfig()
    {
        switch (_configService.GetConfig().Theme)
        {
            case ETheme.System:
                _themeService.SetTheme(Win32Helper.GetWindowsTheme() ? Wpf.Ui.Appearance.ThemeType.Dark : Wpf.Ui.Appearance.ThemeType.Light);
                break;
            case ETheme.Dark:
                _themeService.SetTheme(Wpf.Ui.Appearance.ThemeType.Dark);
                break;
            case ETheme.Light:
                _themeService.SetTheme(Wpf.Ui.Appearance.ThemeType.Light);
                break;
            default:
                break;
        }
        if (_configService.GetConfig().LanguageName == string.Empty)
        {
            _languageService.SelectLanguage();
            _configService.GetConfig().LanguageName = _languageService.GetKey("file.language");
            _configService.Save();
        }
        else
        {
            _languageService.SelectLanguage(_configService.GetConfig().LanguageName, false);
        }
    }
    private async Task HandleActivationAsync()
    {
        await Task.CompletedTask;

        if (!Application.Current.Windows.OfType<Container>().Any())
        {
            _navigationWindow = (_serviceProvider.GetService(typeof(INavigationWindow)) as INavigationWindow)!;
            _navigationWindow!.ShowWindow();
        }

        LoadConfig();

        await Task.CompletedTask;
    }
}
