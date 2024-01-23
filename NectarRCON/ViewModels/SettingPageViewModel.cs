using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NectarRCON.Core.Helper;
using NectarRCON.Interfaces;
using NectarRCON.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using NectarRCON.Dp;
using Wpf.Ui.Mvvm.Contracts;

namespace NectarRCON.ViewModels;
public partial class SettingPageViewModel : ObservableObject
{
    private bool _isLoaded = false;
    private readonly ILanguageService _languageService;
    private readonly IConfigService _configService;
    private readonly IThemeService _themeService;
    private readonly RconSettingsDp _rconSettingsDp = DpFile.LoadSingleton<RconSettingsDp>();

    [ObservableProperty]
    private int _languageSelectedIndex = -1;
    [ObservableProperty]
    private int _themeSelectedIndex = -1;
    
    [ObservableProperty]
    private bool _rconAutoReconnect;
    
    [ObservableProperty]
    private bool _isKeepConnectionWindowOpen;

    [ObservableProperty]
    private ObservableCollection<string> _rconEncoding = [];
    
    [ObservableProperty]
    private string _selectedRconEncoding;

    [ObservableProperty]
    private ObservableCollection<string> _languages = [];
    
    public SettingPageViewModel()
    {
        _languageService = App.GetService<ILanguageService>();
        _configService = App.GetService<IConfigService>();
        _themeService = App.GetService<IThemeService>();

        RconAutoReconnect = _rconSettingsDp.AutoReconnect;
        IsKeepConnectionWindowOpen = _rconSettingsDp.IsKeepConnectionWindowOpen;
        
        RconEncoding.Clear();
        foreach (var encoding in Enum.GetNames(typeof(RconEncoding)))
        {
            RconEncoding.Add(encoding);
        }
        
        SelectedRconEncoding = _rconSettingsDp.Encoding.ToString();
    }

    partial void OnRconAutoReconnectChanged(bool value)
    {
        _rconSettingsDp.AutoReconnect = value;
        _rconSettingsDp.Save();
    }

    partial void OnIsKeepConnectionWindowOpenChanged(bool value)
    {
        _rconSettingsDp.IsKeepConnectionWindowOpen = value;
        _rconSettingsDp.Save();
    }

    partial void OnSelectedRconEncodingChanged(string value)
    {
        _rconSettingsDp.Encoding = Enum.GetValues<RconEncoding>().FirstOrDefault(e => e.ToString() == value);
        _rconSettingsDp.Save();
    }

    [RelayCommand]
    public void PageLoad(RoutedEventArgs e)
    {
        Languages.Clear();
        foreach (var language in _languageService.GetLanguages())
        {
            Languages.Add(language.Value["file.name"].ToString() ?? "NullName");
        }
        foreach (var language in _languageService.GetLanguages())
        {
            LanguageSelectedIndex++;
            if (language.Key == _configService.GetConfig().LanguageName)
                break;
        }
        ThemeSelectedIndex = (int)_configService.GetConfig().Theme;
        _isLoaded = true;
    }
    [RelayCommand]
    public void Exit()
    {
        _isLoaded = false;
    }
    [RelayCommand]
    public void ThemeSelectionChange()
    {
        if (!_isLoaded)
            return;
        _configService.GetConfig().Theme = (ETheme)_themeSelectedIndex;
        _configService.Save();
        switch ((ETheme)_themeSelectedIndex)
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
    }
    [RelayCommand]
    public void LanguageSelectionChange(SelectionChangedEventArgs e)
    {
        if (!_isLoaded)
            return;
        if (e.AddedItems.Count == 0 || null == e.AddedItems[0])
            return;
        KeyValuePair<string, ResourceDictionary>? lang = _languageService.GetLanguages().Where(l =>
        {
            return (l.Value["file.name"].ToString() ?? "NullName") == e.AddedItems[0]!.ToString();
        }).FirstOrDefault();
        if (null == lang.Value.Value) return;
        _configService.GetConfig().LanguageName = lang.Value.Key;
        _configService.Save();
        _languageService.SelectLanguage(lang.Value.Value["file.name"].ToString() ?? "NullName", true);
    }

}