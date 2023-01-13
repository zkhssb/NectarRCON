using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NectarRCON.Interfaces;
using NectarRCON.Models;
using NectarRCON.Services;
using NectarRCON.Windows;
using System.Windows;
using Wpf.Ui.Controls;

namespace NectarRCON.ViewModels;
public partial class EditPasswordWindowViewModel:ObservableObject
{
    private readonly IServerPasswordService _serverPasswordService;
    private readonly ServerInformation _serverInformation;
    private EditPasswordWindow? _window = null;

    [ObservableProperty]
    private string _password = string.Empty;
    [ObservableProperty]
    private bool _isEmpty = true;
    public EditPasswordWindowViewModel()
    {
        _serverPasswordService = App.GetService<IServerPasswordService>();
        _serverInformation = _serverPasswordService.GetSelect()!;
        var password = _serverPasswordService.Get(_serverInformation);
        if(password != null)
        {
            Password = password.Password;
            IsEmpty = string.IsNullOrEmpty(Password);
        }
        else
        {
            Password = string.Empty;
            IsEmpty = true;
        }
    }
    public void SetWindow(EditPasswordWindow window)
    {
        _window= window;
    }
    [RelayCommand]
    public void TextChange()
    {
        IsEmpty = string.IsNullOrEmpty(Password);
    }
    [RelayCommand]
    public void SetPassword()
    {
        _serverPasswordService.Set(_serverInformation, Password, IsEmpty);
        _serverPasswordService.Save();
        _window?.Close();
    }
}