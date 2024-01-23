using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using NectarRCON.Interfaces;
using NectarRCON.Models;
using NectarRCON.Rcon;
using NectarRCON.Services;
using NectarRCON.Views.Pages;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Wpf.Ui.Mvvm.Contracts;
using MessageBox = System.Windows.MessageBox;
using TextBox = Wpf.Ui.Controls.TextBox;

namespace NectarRCON.ViewModels;

public partial class MainPageViewModel : ObservableObject
{
    private readonly ILogService _logService;
    private readonly IServerPasswordService _serverPasswordService;
    private IRconConnection _rconConnectService;
    private readonly INavigationService _navigationService;
    private readonly ILanguageService _languageService;
    private readonly IRconConnectionInfoService _rconConnectionInfoService;
    private readonly IConnectingDialogService _connectingDialogService;
    private readonly IMessageBoxService _messageBoxService;

    private MainPage? _page;
    private TextBox? _logTextBox;

    [ObservableProperty] private string _commandText = string.Empty;
    [ObservableProperty] private string _logText = string.Empty;

    public MainPageViewModel()
    {
        _logService = App.GetService<ILogService>();
        _serverPasswordService = App.GetService<IServerPasswordService>();
        _navigationService = App.GetService<INavigationService>();
        _languageService = App.GetService<ILanguageService>();
        _rconConnectionInfoService = App.GetService<IRconConnectionInfoService>();
        _messageBoxService = App.GetService<IMessageBoxService>();
        _connectingDialogService = App.GetService<IConnectingDialogService>();
        WeakReferenceMessenger.Default.Register<ClearLogValueMessage>(this, OnClear);

        // 选择连接服务
        _rconConnectService = _rconConnectionInfoService.HasMultipleInformation
            ? App.GetService<IRconConnection>(typeof(RconMultiConnection))
            : App.GetService<IRconConnection>(typeof(RconSingleConnection));
    }

    private void OnClear(object sender, ClearLogValueMessage msg)
    {
        _logService.Clear();
        LogText = string.Empty;
    }

    private void OnMessage(ServerInformation info, string msg)
    {
        string logMsg = string.IsNullOrEmpty(msg)
            ? _languageService.GetKey("ui.main_page.successful")
            : msg;
        LogText += _logService.Log($"{info.Name}:" + logMsg);
        _logTextBox?.ScrollToEnd();
    }

    private void OnClosed(ServerInformation info)
    {
        LogText += _logService.Log($"{info.Name}\t{_languageService.GetKey("text.server.closed")}");
    }

    [RelayCommand]
    private async void Load(RoutedEventArgs e)
    {
        try
        {
            _connectingDialogService.Show();
            // 选择连接服务
            _rconConnectService = _rconConnectionInfoService.HasMultipleInformation
                ? App.GetService<IRconConnection>(typeof(RconMultiConnection))
                : App.GetService<IRconConnection>(typeof(RconSingleConnection));

            WeakReferenceMessenger.Default.Send(new MainPageLoadValueMessage()
            {
                IsLoaded = true,
            });

            _page = e.Source as MainPage;
            _logTextBox = (TextBox)LogicalTreeHelper.FindLogicalNode(_page, "LogText");

            LogText = string.Empty;
            LogText = _logService.GetText();
            LogText += _logService.Log(_languageService.GetKey("text.server.start"));
            _rconConnectService.OnConnected += OnConnected;
            _rconConnectService.OnMessage += OnMessage;
            _rconConnectService.OnClosed += OnClosed;
            await Task.Run(_rconConnectService.Connect);
        }
        catch (SocketException ex)
        {
            _messageBoxService.Show(_languageService.GetKey("text.server.connect.fail.text")
                    .Replace("\\n", "\n")
                    .Replace("%s", ex.Message), _languageService.GetKey("text.error"), MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
        catch (AuthenticationException ex)
        {
            _messageBoxService.Show(ex.Message + _languageService.GetKey("text.server.connect.auth_fail")
                    .Replace("\\n", "\n"), _languageService.GetKey("text.error"), MessageBoxButton.OK,
                MessageBoxImage.Error);

            // 如果认证失败 就根据当前模式返回对应页面
            _navigationService.Navigate(_rconConnectionInfoService.HasMultipleInformation
                ? typeof(GroupPage)
                : typeof(ServersPage));
        }
        finally
        {
            _connectingDialogService.Close();
        }

        // 当只有一个服务器时IsConnected会返回单个客户端的连接状态
        // 当有多个服务器时只要有一个客户端在线,IsConnected就会返回True
        if (!_rconConnectService.IsConnected())
        {
            _navigationService.Navigate(2);
        }
    }

    private void OnConnected(ServerInformation info)
    {
        LogText += _logService.Log($"$ {info.Name}\t{_languageService.GetKey("text.server.connected")}");
    }

    [RelayCommand]
    private void Exit()
    {
        WeakReferenceMessenger.Default.Send(new MainPageLoadValueMessage()
        {
            IsLoaded = false,
        });
        if (_rconConnectService.IsConnected())
            _rconConnectService.Close();
        _rconConnectService.OnMessage -= OnMessage;
        _rconConnectService.OnClosed -= OnClosed;
        _rconConnectService.OnConnected -= OnConnected;
    }

    [RelayCommand]
    private void Run()
    {
        if (_rconConnectService.IsConnected())
        {
            LogText += _logService.Log($"> {CommandText}");
            _logTextBox?.ScrollToEnd();
            _rconConnectService.Send(CommandText);
            CommandText = string.Empty;
        }
        else
        {
            _rconConnectService.Close();
            MessageBox.Show(_languageService.GetKey("text.server.not_connect.text"),
                _languageService.GetKey("text.error"), MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    [RelayCommand]
    private void KeyDown(KeyEventArgs e)
    {
        var textBox = (System.Windows.Controls.TextBox)e.Source;
        _commandText = textBox.Text;
        if (e.Key == Key.Enter)
        {
            Run();
        }
    }
}