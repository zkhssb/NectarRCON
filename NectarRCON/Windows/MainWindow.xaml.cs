using NectarRCON.Interfaces;
using NectarRCON.ViewModels;
using NectarRCON.Views.Pages;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Wpf.Ui.Common.Interfaces;
using Wpf.Ui.Controls;
using Wpf.Ui.Controls.Interfaces;
using Wpf.Ui.Mvvm.Contracts;
using Wpf.Ui.Mvvm.Services;

namespace NectarRCON
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : INavigationWindow, INavigableView<MainWindowViewModel>
    {
        private readonly INavigationService _navigationService;
        private readonly IThemeService _themeService;
        public MainWindowViewModel ViewModel
        {
            get;
        }

        MainWindowViewModel INavigableView<MainWindowViewModel>.ViewModel => ViewModel;
        public MainWindow(INavigationService navigationService, IThemeService themeService, MainWindowViewModel viewModel, IConnectingDialogService connectingDialogService)
        {
            InitializeComponent();
            DataContext = this;
            _navigationService = navigationService;
            _themeService = themeService;
            _navigationService.SetNavigationControl(RootNavigation);
            ViewModel = viewModel;
            connectingDialogService.SetDialog(ConnectingDialog);
        }
        #region INavigationWindow methods
        public Frame GetFrame()
            => MainFrame;

        public INavigation GetNavigation()
            => RootNavigation;

        public bool Navigate(Type pageType)
            => RootNavigation.Navigate(pageType);

        public void SetPageService(IPageService pageService)
            => RootNavigation.PageService = pageService;

        public void ShowWindow()
            => Show();

        public void CloseWindow()
            => Close();
        #endregion
    }
}
