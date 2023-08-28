using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NectarRCON.Interfaces;
using NectarRCON.Services;
using System;
using Wpf.Ui.Mvvm.Contracts;

namespace NectarRCON.ViewModels;
public partial class AddGroupPageViewModel:ObservableObject
{
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(AddCommand))]
    private string? _groupName;

    private INavigationService _navigationService;
    private IMessageBoxService _messageBoxService;
    private IGroupService _groupService;

    public AddGroupPageViewModel()
    {
        GroupName = string.Empty;
        _navigationService = App.GetService<INavigationService>();
        _messageBoxService = App.GetService<IMessageBoxService>();
        _groupService = App.GetService<IGroupService>();
    }

    private bool CanAdd()
        => !string.IsNullOrWhiteSpace(GroupName);

    [RelayCommand(CanExecute = nameof(CanAdd))]
    public void Add()
    {
        try
        {
            _groupService.Add(new Models.Group()
            {
                Id = Guid.NewGuid().ToString(),
                Name = GroupName!,
                Servers = new()
            });
            _navigationService.Navigate(4);
        }catch(Exception ex)
        {
            _messageBoxService.Show(ex, string.Empty);
        }
    }

    [RelayCommand]
    public void Cancel()
    {
        _navigationService.Navigate(4);
    }

    [RelayCommand]
    public void Load()
    {
        GroupName = null;
    }
}
