using System;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NectarRCON.Interfaces;
using NectarRCON.Models;
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
    private ObservableCollection<string> _rconAdapterList;
    [ObservableProperty]
    private string _port = string.Empty;
    [ObservableProperty]
    private string _address = string.Empty;
    [ObservableProperty]
    private int _selectedAdapterIndex;
    public EditServerWindowViewModel()
    {
        _languageService = App.GetService<ILanguageService>();
        _serverPasswordService = App.GetService<IServerPasswordService>();
        _serverInformationService = App.GetService<IServerInformationService>();
        RconAdapterList = new ObservableCollection<string>(Enum.GetValues<RconAdapter>()
            .Select(adapter => _languageService.GetKey(adapter.ToAdapterString())).ToList());
    }

    [RelayCommand]
    private void Load(RoutedEventArgs e)
    {
        _window = e.Source as UiWindow;
        SelectServer = _serverPasswordService.GetSelect();
        Port = _selectServer?.Port.ToString() ?? "0";
        Address = _selectServer?.Address ?? "localhost";
        SelectedAdapterIndex = Enum.GetValues<RconAdapter>().ToList().IndexOf(_selectServer?.Adapter?.ToAdapter() ?? RconAdapter.Minecraft);
    }
    [RelayCommand]
    public void Exit()
    {
        _window?.Close();
    }
    [RelayCommand]
    private void Ok()
    {
        Address = Address.Trim();
        if (string.IsNullOrWhiteSpace(_selectServer?.Address) || string.IsNullOrEmpty(Address))
            MessageBox.Show(_languageService.GetKey("ui.add_server_window.null_text"), _languageService.GetKey("text.error"), MessageBoxButton.OK, MessageBoxImage.Error);
        else
        {
            _selectServer.Port = ushort.Parse(Port);
            _selectServer.Address = Address;
            _selectServer.Adapter = ((RconAdapter)SelectedAdapterIndex).ToAdapterString();
            _serverInformationService.Update(_selectServer.Name, _selectServer);
            _serverInformationService.Save();
            Exit();
        }
    }
}