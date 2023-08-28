using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using NectarRCON.Interfaces;
using NectarRCON.Models;
using System.Collections.ObjectModel;
using Wpf.Ui.Common;
using Wpf.Ui.Controls;
using Wpf.Ui.Controls.Interfaces;
using Wpf.Ui.Mvvm.Contracts;

namespace NectarRCON.ViewModels
{
    public partial class MainWindowViewModel : ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<INavigationControl> _navigationItems = new();
        private readonly INavigationService _navigationService;
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
            WeakReferenceMessenger.Default.Register<MainPageLoadValueMessage>(this, OnMainPageChange);
        }
        public void OnMainPageChange(object sender, MainPageLoadValueMessage message)
        {
            MainPageIsLoaded = message.IsLoaded;
        }
        [RelayCommand]
        public void OnLoad()
        {
            _navigationService.Navigate(2);
        }
        [RelayCommand]
        public void ClearButtonClick()
        {
            WeakReferenceMessenger.Default.Send(new ClearLogValueMessage());
        }
        [RelayCommand]
        public void ChangePage(string index)
        {
            _navigationService.Navigate(int.Parse(index));
        }
    }
}
