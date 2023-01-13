using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NectarRCON.Interfaces;
using NectarRCON.Models;
using NectarRCON.Windows;
using System.Windows;

namespace NectarRCON.ViewModels;
public partial class AddServerWindowViewModel:ObservableObject
{
    private readonly IServerInformationService _serverInformationService;
    private readonly ILanguageService _languageService;
    private AddServerWindow? _serverWindow = null;
    [ObservableProperty]
    private string _serverName = "Rcon";
    [ObservableProperty]
    private string _serverAddress = "localhost";
    [ObservableProperty]
    private string _serverPort = "25575";
    public AddServerWindowViewModel(IServerInformationService serverInformationService, ILanguageService languageService)
    {
        _serverInformationService = serverInformationService;
        _languageService = languageService;
    }
    public void SetWindow(AddServerWindow window)
    {
        _serverWindow = window;
    }
    [RelayCommand]
    public void Ok()
    {
        if(string.IsNullOrWhiteSpace(_serverName) || string.IsNullOrWhiteSpace(_serverAddress))
        {
            MessageBox.Show(_languageService.GetKey("ui.add_server_window.null_text"), _languageService.GetKey("text.error"), MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }
        ServerInformation information = new ServerInformation()
        {
            Name = _serverName,
            Address = _serverAddress,
            Port = ushort.Parse(_serverPort)
        };
        if (_serverInformationService.ServerIsExist(information.Name))
        {
            MessageBox
                .Show(_languageService.GetKey("ui.add_server_window.already_exist")
                , _languageService.GetKey("text.error"), MessageBoxButton.OK, MessageBoxImage.Error);
        }
        else
        {
            _serverInformationService.AddServer(information);
            _serverInformationService.Save();
            _serverWindow?.Close();
        }
    }
}