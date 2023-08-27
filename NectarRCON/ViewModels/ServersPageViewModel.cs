using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NectarRCON.Interfaces;
using NectarRCON.Models;
using NectarRCON.Rcon;
using NectarRCON.Windows;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using Wpf.Ui.Controls;
using Wpf.Ui.Mvvm.Contracts;

namespace NectarRCON.ViewModels;
public partial class ServersPageViewModel : ObservableObject
{
    private readonly IServerInformationService _serverInformationService;
    private readonly IRconConnection _conConnectService;
    private readonly IConnectingDialogService _connectingDialogService;
    private readonly ILanguageService _languageService;
    private readonly IServerPasswordService _serverPasswordService;
    private readonly ILogService _logService;
    private readonly INavigationService _navigationService;

    [ObservableProperty]
    private ListCollectionView _serverCollectionView;
    private string _filterName = string.Empty;
    private ServerInformation? _selectServer = null;
    public ServersPageViewModel(IServerInformationService informationService, IRconConnection conConnectService, IConnectingDialogService connectingDialogService, ILanguageService languageService, IServerPasswordService serverPasswordService, ILogService logService, INavigationService navigationService)
    {
        _serverInformationService = informationService;
        _serverCollectionView = new(informationService.GetServers());
        _serverCollectionView.Filter += (s) =>
        {
            if (_filterName == string.Empty)
                return true;
            ServerInformation info = (ServerInformation)s;
            return info.Name.Contains(_filterName);
        };
        _serverCollectionView.Refresh();
        _conConnectService = conConnectService;
        _connectingDialogService = connectingDialogService;
        _languageService = languageService;
        _serverPasswordService = serverPasswordService;
        _logService = logService;
        _navigationService = navigationService;
    }
    [RelayCommand]
    public void AddServer()
    {
        AddServerWindow addServer = App.GetService<AddServerWindow>();
        addServer.ShowDialog();
        _serverCollectionView.Refresh();
    }
    [RelayCommand]
    public void FilterTextChanged(TextChangedEventArgs e)
    {
        var box = (System.Windows.Controls.TextBox)e.Source;
        _filterName = box.Text.ToString() ?? string.Empty;
        _serverCollectionView.Refresh();
    }
    [RelayCommand]
    public void MouseEnterCard(RoutedEventArgs e)
    {
        if (e.Source == null)
            return;
        CardAction cardAction = (CardAction)e.Source;
        var nameText = (System.Windows.Controls.TextBlock)LogicalTreeHelper.FindLogicalNode(cardAction, "Name");
        _selectServer = _serverInformationService.GetServer(nameText.Text);
    }
    private ContextMenu? GetRoot(DependencyObject menu)
    {
        if (menu is CardAction) { return (ContextMenu)menu; }

        DependencyObject root = menu;
        for (int i = 0; i < 20; i++)
        {
            root = VisualTreeHelper.GetParent(root);
            if (root is ContextMenu)
            {
                return (ContextMenu)root;
            }
        }

        return null;
    }
    private string GetRootName(ContextMenu? root)
    {
        if (null == root)
            return string.Empty;
        StackPanel stackPanel = (StackPanel)root.Tag;
        var nameText = (System.Windows.Controls.TextBlock)LogicalTreeHelper.FindLogicalNode(stackPanel, "Name");
        return nameText.Text ?? string.Empty;
    }
    private ServerInformation? GetServerInformation(RoutedEventArgs e)
    {
        if (e.Source == null || e.Source is not DependencyObject)
            return null;
        string name = GetRootName(GetRoot((DependencyObject)e.Source));
        return _serverInformationService.GetServer(name);
    }
    [RelayCommand]
    public async void MenuConnect(RoutedEventArgs e)
    {
        var serverInfo = GetServerInformation(e);
        if (null == serverInfo) return;
        await Connect(serverInfo);
    }
    [RelayCommand]
    public async void CardClick(RoutedEventArgs e)
    {
        CardAction card = (CardAction)e.Source;
        var nameText = (TextBlock)LogicalTreeHelper.FindLogicalNode(card, "Name");
        ServerInformation? server = _serverInformationService.GetServer(nameText.Text);
        if (null != server)
        {
            await Connect(server);
        }
    }
    private void EditPass(ServerInformation info)
    {
        _serverPasswordService.Select(info);
        EditPasswordWindow window = new EditPasswordWindow();
        window.ShowDialog();
    }

    [RelayCommand]
    public void EditPassword(RoutedEventArgs e)
    {
        var serverInfo = GetServerInformation(e);
        if (null == serverInfo) return;
        EditPass(serverInfo);
    }
    [RelayCommand]
    public void MenuEdit(RoutedEventArgs e)
    {
        var serverInfo = GetServerInformation(e);
        if (null == serverInfo) return;
        _serverPasswordService.Select(serverInfo);
        EditServerWindow window = new EditServerWindow();
        window.ShowDialog();
        _serverCollectionView.Refresh();
    }
    [RelayCommand]
    public void MenuDelete(RoutedEventArgs e)
    {
        var serverInfo = GetServerInformation(e);
        if (null == serverInfo) return;
        var result = System.Windows.MessageBox.Show(_languageService.GetKey("ui.server_page.confirm"), _languageService.GetKey("ui.server_page.menu.delete"), MessageBoxButton.YesNo, MessageBoxImage.Question);
        if (result == MessageBoxResult.Yes)
        {
            _serverInformationService.RemoveServer(serverInfo.Name);
            _serverInformationService.Save();
            _serverPasswordService.Remove(serverInfo);
            _serverPasswordService.Save();
            _serverCollectionView.Refresh();
        }
    }
    public async Task Connect(ServerInformation information)
    {
        if (_conConnectService.IsConnecting())
            return;
        try
        {
            _connectingDialogService.Show();
            if (_conConnectService.IsConnected())
            {
                _conConnectService.Close();
            }
            var server = _serverPasswordService.Get(information);
            if (server == null)
            {
                EditPass(information);
            }
            else if (server.Password == null && !server.IsEmpty)
            {
                EditPass(information);
            }
            //await _conConnectService.ConnectAsync(information);
            _serverPasswordService.Select(information);
            if (_conConnectService.IsConnected())
                _navigationService.Navigate(0);
        }
        catch (SocketException ex)
        {
            System.Windows.MessageBox.Show(_languageService.GetKey("text.server.connect.fail.text")
                .Replace("\\n", "\n")
                .Replace("%s", ex.Message), _languageService.GetKey("text.error"), MessageBoxButton.OK, MessageBoxImage.Error);
        }
        catch (AuthenticationException)
        {
            System.Windows.MessageBox.Show(_languageService.GetKey("text.server.connect.auth_fail")
            .Replace("\\n", "\n"), _languageService.GetKey("text.error"), MessageBoxButton.OK, MessageBoxImage.Error);
        }
        finally
        {
            _connectingDialogService.Close();
        }
    }
}
