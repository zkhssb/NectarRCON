using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NectarRCON.Interfaces;
using NectarRCON.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Wpf.Ui.Controls;
using MessageBox = System.Windows.MessageBox;

namespace NectarRCON.ViewModels;
public partial class EditServerWindowViewModel : ObservableObject
{
    private readonly ILanguageService _languageService;
    private readonly IServerPasswordService _serverPasswordService;
    private readonly IServerInformationService _serverInformationService;
    private UiWindow? _window;
    [ObservableProperty]
    private ServerInformation? _selectServer;
    [ObservableProperty]
    private string _port = string.Empty;
    [ObservableProperty]
    private string _address = string.Empty;
    public EditServerWindowViewModel()
    {
        _languageService = App.GetService<ILanguageService>();
        _serverPasswordService = App.GetService<IServerPasswordService>();
        _serverInformationService = App.GetService<IServerInformationService>();
    }

    [RelayCommand]
    public void Load(RoutedEventArgs e)
    {
        _window = e.Source as UiWindow;
        SelectServer = _serverPasswordService.GetSelect();
        Port = _selectServer?.Port.ToString() ?? "0";
        Address = _selectServer?.Address.ToString() ?? "localhost";
    }
    [RelayCommand]
    public void Exit()
    {
        _window?.Close();
    }
    [RelayCommand]
    public void Ok()
    {
        if (string.IsNullOrWhiteSpace(_selectServer?.Address))
            MessageBox.Show(_languageService.GetKey("ui.add_server_window.null_text"), _languageService.GetKey("text.error"), MessageBoxButton.OK, MessageBoxImage.Error);
        else
        {
            _selectServer.Port = ushort.Parse(Port);
            _selectServer.Address = Address;
            _serverInformationService.Update(_selectServer.Name, _selectServer);
            _serverInformationService.Save();
            Exit();
        }
    }
}