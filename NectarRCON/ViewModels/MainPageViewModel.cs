using System;
using System.Collections.ObjectModel;
using System.Linq;
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
using NectarRCON.Dp;
using Serilog;
using Wpf.Ui.Mvvm.Contracts;
using MessageBox = System.Windows.MessageBox;
using TextBox = Wpf.Ui.Controls.TextBox;

namespace NectarRCON.ViewModels;

public partial class MainPageViewModel : ObservableObject
{
    private static readonly RconSettingsDp RconSettings = DpFile.LoadSingleton<RconSettingsDp>();
    private readonly ILogService _logService;
    private readonly IHistoryService _historyService;
    private readonly IServerPasswordService _serverPasswordService;
    private IRconConnection _rconConnectService;
    private readonly INavigationService _navigationService;
    private readonly ILanguageService _languageService;
    private readonly IRconConnectionInfoService _rconConnectionInfoService;
    private readonly IConnectingDialogService _connectingDialogService;
    private readonly IMessageBoxService _messageBoxService;

    private MainPage? _page;
    private TextBox? _logTextBox;
    private HistoryNode? _historyNode;

    [ObservableProperty] private string _commandText = string.Empty;
    [ObservableProperty] private string _logText = string.Empty;
    [ObservableProperty] private bool _isMultipleConnection;
    [ObservableProperty] private bool _isDisconnection;
    [ObservableProperty] private ObservableCollection<string> _commandList = [];
    public MainPageViewModel()
    {
        _logService = App.GetService<ILogService>();
        _historyService = App.GetService<IHistoryService>();
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

        IsMultipleConnection = _rconConnectionInfoService.HasMultipleInformation;
    }

    private void OnClear(object sender, ClearLogValueMessage msg)
    {
        _logService.Clear();
        LogText = string.Empty;
    }

    private void OnMessage(ServerInformation info, string msg)
    {
        Log.Information("[OnMessage] {name}({adapter}) -> {msg}", info.Name, string.IsNullOrEmpty(info.Adapter) ? "TCPRcon" : info.Adapter, string.IsNullOrEmpty(msg) ? "$empty$" : msg);
        string logMsg = string.IsNullOrEmpty(msg)
            ? _languageService.GetKey("ui.main_page.successful")
            : msg;
        LogText += _logService.Log($"{info.Name}:" + logMsg);
        _logTextBox?.ScrollToEnd();
    }

    private void OnClosed(ServerInformation info)
    {
        LogText += _logService.Log($"{info.Name} {_languageService.GetKey("text.server.closed")}");
        IsDisconnection = !_rconConnectService.IsConnected();
    }

    [RelayCommand]
    private async void Load(RoutedEventArgs e)
    {
        // GetLogs
        LogText = string.Empty;
        LogText = _logService.GetText();

        _page = e.Source as MainPage;
        await ConnectAsync();
    }

    [RelayCommand]
    private async void ReConnect()
    {
        Log.Information($"[ReConnectCommand] 正在准备重连");
        if (_rconConnectService.IsConnected())
            _rconConnectService.Close();
        IsDisconnection = false;
        await ConnectAsync();
    }

    private async Task ConnectAsync()
    {
        Log.Information($"[ConnectAsync] 准备连接到服务器");

        IsMultipleConnection = _rconConnectionInfoService.HasMultipleInformation;
        _rconConnectService.OnConnected -= OnConnected;
        _rconConnectService.OnMessage -= OnMessage;
        _rconConnectService.OnClosed -= OnClosed;
        await Task.CompletedTask;
        try
        {
            _connectingDialogService.Show();
            // 选择连接服务
            _rconConnectService = _rconConnectionInfoService.HasMultipleInformation
                ? App.GetService<IRconConnection>(typeof(RconMultiConnection))
                : App.GetService<IRconConnection>(typeof(RconSingleConnection));

            WeakReferenceMessenger.Default.Send(new MainPageLoadValueMessage
            {
                IsLoaded = true,
            });

            Log.Information($"[ConnectAsync] 连接服务: {_rconConnectService.GetType().FullName}, 是否为多连接: {IsMultipleConnection}");

            _logTextBox = (TextBox)LogicalTreeHelper.FindLogicalNode(_page, "LogText");
            LogText += _logService.Log(_languageService.GetKey("text.server.start"));
            _logTextBox?.ScrollToEnd();
            _rconConnectService.OnConnected += OnConnected;
            _rconConnectService.OnMessage += OnMessage;
            _rconConnectService.OnClosed += OnClosed;
            await Task.Run(_rconConnectService.Connect);
        }
        catch (AuthenticationException ex)
        {
            Log.Error($"[ConnectAsync] 认证失败: {ex.Message}");
            var msg = ex.Message + _languageService.GetKey("text.server.connect.auth_fail")
                .Replace("\\n", "\n");
            _messageBoxService.Show(msg, _languageService.GetKey("text.error"), MessageBoxButton.OK,
                MessageBoxImage.Error);
            LogText += _logService.Log(msg);

            // 如果认证失败 就根据当前模式返回对应页面
            _navigationService.Navigate(_rconConnectionInfoService.HasMultipleInformation
                ? typeof(GroupPage)
                : typeof(ServersPage));
        }
        catch (Exception ex)
        {
            Log.Error($"[ConnectAsync] 连接遇到错误: {ex}");

            var msg = _languageService.GetKey("text.server.connect.fail.text")
                .Replace("\\n", "\n")
                .Replace("%s", ex.Message);
            _messageBoxService.Show(msg, _languageService.GetKey("text.error"), MessageBoxButton.OK,
                MessageBoxImage.Error);
            LogText += _logService.Log(msg);
            _logTextBox?.ScrollToEnd();
        }
        finally
        {
            _connectingDialogService.Close();
        }

        // 当只有一个服务器时IsConnected会返回单个客户端的连接状态
        // 当有多个服务器时只要有一个客户端在线,IsConnected就会返回True
        if (!_rconConnectService.IsConnected() && !RconSettings.IsKeepConnectionWindowOpen)
        {
            _navigationService.Navigate(2);
        }

        IsDisconnection = !_rconConnectService.IsConnected();
    }

    partial void OnIsDisconnectionChanged(bool value)
    {
        Log.Information("当前客户端状态: {0}", _rconConnectService.IsConnected() ? "在线" : "离线");
    }

    private void OnConnected(ServerInformation info)
    {
        Log.Information("[OnConnected] {name}({adapter})", info.Name, string.IsNullOrEmpty(info.Adapter) ? "TCPRcon" : info.Adapter);
        LogText += _logService.Log($"$ {info.Name} {_languageService.GetKey("text.server.connected")}");
        IsDisconnection = false;
        
        // 获取适配器的命令提示
        // 只有单连接支持获取命令帮助,组服务器不知道都是些什么适配器
        if (_rconConnectService is RconSingleConnection singleConnection)
        {
            CommandList = new ObservableCollection<string>(singleConnection.GetCommands().ToList());
        }
    }

    [RelayCommand]
    private void BackHome()
    {
        _navigationService.Navigate(_rconConnectionInfoService.HasMultipleInformation
            ? typeof(GroupPage)
            : typeof(ServersPage));
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
        IsDisconnection = false; // 重置状态 此时没有任何连接
    }

    [RelayCommand]
    private void Run()
    {
        if(CommandText.StartsWith("/")) CommandText = CommandText[1..];
        if(string.IsNullOrWhiteSpace(CommandText)) return;
        
        Log.Information("[Run] {0}", CommandText);
        if (_rconConnectService.IsConnected())
        {
            LogText += _logService.Log($"> {CommandText}");
            _logTextBox?.ScrollToEnd();
            _rconConnectService.Send(CommandText);
            CommandText = string.Empty;
            _page?.CloseCommandInputBoxPopup();
        }
        else
        {
            IsDisconnection = true;
            _rconConnectService.Close();
            MessageBox.Show(_languageService.GetKey("text.server.not_connect.text"),
                _languageService.GetKey("text.error"), MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    [RelayCommand]
    private void KeyDown(KeyEventArgs e)
    {
        var textBox = (System.Windows.Controls.TextBox)e.Source;
        switch (e.Key)
        {
            case Key.Enter:
            {
                var text = textBox.Text.Trim();
                if (string.IsNullOrEmpty(text))
                {
                    return;
                }
                _commandText = text;
                _historyNode = _historyService.InputCmd(_commandText);
                Run();
                break;
            }
            case Key.Up:
                _historyNode = _historyService.Prev(_historyNode);
                textBox.Text = _historyNode?.Cmd ?? string.Empty;
                textBox.Select(textBox.Text?.Length ?? 0, 0);
                break;
            case Key.Down:
                _historyNode = _historyService.Next(_historyNode);
                textBox.Text = _historyNode?.Cmd ?? string.Empty;
                textBox.Select(textBox.Text?.Length ?? 0, 0);
                break;
        }
    }
}