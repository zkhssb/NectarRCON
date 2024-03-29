﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NectarRCON.Interfaces;
using NectarRCON.Models;
using NectarRCON.Windows;
using System.Windows;

namespace NectarRCON.ViewModels;
public partial class AddServerWindowViewModel : ObservableObject
{
    private readonly IServerInformationService _serverInformationService;
    private readonly ILanguageService _languageService;
    private AddServerWindow? _serverWindow;
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
    private void Ok()
    {
        ServerAddress = ServerAddress.Trim();
        if (string.IsNullOrWhiteSpace(ServerName) || string.IsNullOrWhiteSpace(ServerAddress))
        {
            MessageBox.Show(_languageService.GetKey("ui.add_server_window.null_text"), _languageService.GetKey("text.error"), MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }
        var information = new ServerInformation()
        {
            Name = ServerName,
            Address = ServerAddress,
            Port = ushort.Parse(ServerPort)
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