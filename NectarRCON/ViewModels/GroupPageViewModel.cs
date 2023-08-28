using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NectarRCON.Interfaces;
using NectarRCON.Models;
using NectarRCON.Views.Pages;
using System.Collections.ObjectModel;
using Wpf.Ui.Mvvm.Contracts;

namespace NectarRCON.ViewModels;
public partial class GroupPageViewModel:ObservableObject
{
    [ObservableProperty]
    private ObservableCollection<GroupModel> _groups;
    private readonly IGroupService _groupService;
    private readonly ILanguageService _languageService;
    private readonly IMessageBoxService _messageBoxService;
    private readonly INavigationService _navigationService;
#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。
    public GroupPageViewModel()
#pragma warning restore CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。
    {
        _groupService = App.GetService<IGroupService>();
        _languageService = App.GetService<ILanguageService>();
        _messageBoxService = App.GetService<IMessageBoxService>();
        _navigationService = App.GetService<INavigationService>();
        Groups = new();
    }

    [RelayCommand]
    public void Load()
    {
        Refresh();
    }

    private void Refresh()
    {
        Groups.Clear();
        foreach(Group group in _groupService.GetGroups())
        {
            Groups.Add(new(group.Name, this));
        }
    }

    [RelayCommand]
    public void RemoveGroup(string groupId)
    {
        Group group = _groupService.GetGroup(groupId)!;
        if (_messageBoxService.Show(string.Format(_languageService.GetKey("ui.group.delete_group"), group.Name), _languageService.GetKey("text.delete"), System.Windows.MessageBoxButton.YesNo, System.Windows.MessageBoxImage.Question)
            == System.Windows.MessageBoxResult.Yes)
        {
            _groupService.Delete(groupId);
            Refresh();
        }
    }

    [RelayCommand]
    public void NewGroup()
    {
        _navigationService.Navigate(typeof(AddGroupPage));
    }
}