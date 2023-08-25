using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using NectarRCON.Interfaces;
using NectarRCON.Models;
using NectarRCON.Views.Pages;
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
    private readonly IRconConnection _rconConnectService;
    private readonly INavigationService _navigationService;
    private readonly ILanguageService _languageService;

    private MainPage? _page = null;
    private TextBox? _logTextBox = null;

    [ObservableProperty]
    private string _commandText = string.Empty;
    [ObservableProperty]
    private string _logText = string.Empty;
    public MainPageViewModel()
    {
        _logService = App.GetService<ILogService>();
        _serverPasswordService = App.GetService<IServerPasswordService>();
        _rconConnectService = App.GetService<IRconConnection>();
        _navigationService = App.GetService<INavigationService>();
        _languageService = App.GetService<ILanguageService>();
        WeakReferenceMessenger.Default.Register<ClearLogValueMessage>(this, OnClear);
    }
    public void OnClear(object sender, ClearLogValueMessage msg)
    {
        _logService.Clear();
        LogText = string.Empty;
    }
    private void OnMessage(ServerInformation info, string msg)
    {
        string logMsg = string.IsNullOrEmpty(msg)
            ? _languageService.GetKey("ui.main_page.successful")
            : msg;
        LogText += _logService.Log(logMsg);
        _logTextBox?.ScrollToEnd();
    }
    private void OnClosed(ServerInformation info)
    {
        LogText += _logService.Log(_languageService.GetKey("text.server.closed"));
    }
    [RelayCommand]
    public void Load(RoutedEventArgs e)
    {
        WeakReferenceMessenger.Default.Send(new MainPageLoadValueMessage()
        {
            IsLoaded = true,
        });

        _page = e.Source as MainPage;
        _logTextBox = (TextBox)LogicalTreeHelper.FindLogicalNode(_page, "LogText");

        LogText = string.Empty;
        _logService.SetServer(_serverPasswordService.GetSelect());
        LogText = _logService.GetText();

        if (!_rconConnectService.IsConnected())
        {
            _navigationService.Navigate(2);
        }
        else
        {
            LogText += _logService.Log(_languageService.GetKey("text.server.connected"));
            _logTextBox.ScrollToEnd();
        }

        _rconConnectService.OnMessage += OnMessage;
        _rconConnectService.OnClosed += OnClosed;
    }
    [RelayCommand]
    public void Exit()
    {
        WeakReferenceMessenger.Default.Send(new MainPageLoadValueMessage()
        {
            IsLoaded = false,
        });
        if (_rconConnectService.IsConnected())
            _rconConnectService.Close();
        _rconConnectService.OnMessage -= OnMessage;
        _rconConnectService.OnClosed -= OnClosed;
    }
    [RelayCommand]
    public async Task Run()
    {
        if (string.IsNullOrWhiteSpace(LogText))
            return;

        if (_rconConnectService.IsConnected())
        {
            LogText += _logService.Log($"> {CommandText}");
            _logTextBox?.ScrollToEnd();
            await _rconConnectService.Send(CommandText);
            CommandText = string.Empty;
        }
        else
        {
            _rconConnectService.Close();
            MessageBox.Show(_languageService.GetKey("text.server.not_connect.text"), _languageService.GetKey("text.error"), MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
    [RelayCommand]
    public async Task KeyDown(KeyEventArgs e)
    {
        var textBox = (System.Windows.Controls.TextBox)e.Source;
        _commandText = textBox.Text;
        if (e.Key == Key.Enter)
        {
            await Run();
        }
    }
}