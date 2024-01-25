using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using NectarRCON.Interfaces;
using NectarRCON.Models;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using Serilog;
using Wpf.Ui.Common;
using Wpf.Ui.Controls;
using Wpf.Ui.Controls.Interfaces;
using Wpf.Ui.Mvvm.Contracts;
using MessageBox = System.Windows.MessageBox;

namespace NectarRCON.ViewModels
{
    public partial class MainWindowViewModel : ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<INavigationControl> _navigationItems = new();
        private readonly INavigationService _navigationService;
        private readonly ILanguageService _languageService;
        [ObservableProperty]
        private bool _mainPageIsLoaded;
        public MainWindowViewModel(INavigationService navigationService, ILanguageService languageService)
        {
            NavigationItems = new ObservableCollection<INavigationControl>()
            {
                new NavigationItem
                {
                    Content = "主页",
                    PageTag = "mainPage",
                    PageType = typeof(Views.Pages.MainPage)
                },
                new NavigationItem
                {
                    Content = "设置",
                    PageTag = "settingsPage",
                    PageType= typeof(Views.Pages.SettingPage)
                },
                new NavigationItem
                {
                    Content = "服务器...",
                    PageTag = "serversPage",
                    PageType= typeof(Views.Pages.ServersPage)
                },
                new NavigationItem
                {
                    Content = "关于",
                    PageTag = "aboutPage",
                    PageType= typeof(Views.Pages.AboutPage)
                },
                new NavigationItem
                {
                    Content = "群组",
                    PageTag = "groupPage",
                    PageType= typeof(Views.Pages.GroupPage)
                },
                new NavigationItem
                {
                    Content = "添加服务器",
                    PageTag = "newGroup",
                    PageType= typeof(Views.Pages.AddGroupPage)
                }
            };
            _navigationService = navigationService;
            _languageService = languageService;
            WeakReferenceMessenger.Default.Register<MainPageLoadValueMessage>(this, OnMainPageChange);
        }
        public void OnMainPageChange(object sender, MainPageLoadValueMessage message)
        {
            MainPageIsLoaded = message.IsLoaded;
        }
        [RelayCommand]
        private void OnLoad()
        {
            _navigationService.Navigate(2);
        }
        [RelayCommand]
        private void ClearButtonClick()
        {
            WeakReferenceMessenger.Default.Send(new ClearLogValueMessage());
        }
        [RelayCommand]
        private void ChangePage(string index)
        {
            _navigationService.Navigate(int.Parse(index));
        }

        [RelayCommand]
        private void ClearProgramLogs()
        {
            if (MessageBox.Show(_languageService.GetKey("ui.menu.log.clear_program.ask"), "NectarRcon", MessageBoxButton.YesNo,
                    MessageBoxImage.Question) != MessageBoxResult.Yes) return;
            Log.Information("Clear program logs");
            foreach (var logFile in Directory.GetFiles(Path.Combine(Environment.CurrentDirectory, "logs", "program"), "*.log"))
            {
                try
                {
                    Log.Information("Delete log file: {0}", logFile);
                    File.Delete(logFile);
                }
                catch(Exception ex)
                {
                    Log.Error(ex, "Delete log file failed: {0}", logFile);
                }   
            }
        }
    }
}
