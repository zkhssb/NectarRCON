using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NectarRCON.Interfaces;
using NectarRCON.Rcon;
using NectarRCON.ViewModels;
using NectarRCON.Windows;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Xml.Linq;

namespace NectarRCON.Models;
public partial class GroupModel:ObservableObject
{
    private readonly IGroupService _groupService;

    [ObservableProperty]
    private string _id;

    [ObservableProperty]
    private GroupPageViewModel _baseModel;

    [ObservableProperty]
    private string _name;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ConnectCommand))]
    private ObservableCollection<GroupServerItemModel> _servers = new();


#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。
    public GroupModel(string name, GroupPageViewModel baseModel)
#pragma warning restore CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。
    {
        _groupService = App.GetService<IGroupService>();
        _baseModel = baseModel;
        Name = name;
        Id = _groupService.FindGroup(name)?.Id ?? throw new InvalidOperationException();
        Refresh();
    }

    private void Refresh()
    {
        Group? group = _groupService.GetGroup(_id);
        if (group != null)
        {
            Servers.Clear();
            Name = group.Name;
            foreach (var server in group.Servers)
            {
                Servers.Add(new(server, this));
            }
        }
    }

    [RelayCommand]
    public void ItemRemove(string name)
    {
        Servers.Remove(Servers.Where(s => s.Name == name).First());
        Save();
        ConnectCommand.NotifyCanExecuteChanged();
    }

    private bool ConnectCommandCanExecute()
        => Servers.Count >= 1;

    [RelayCommand(CanExecute = nameof(ConnectCommandCanExecute))]
    public void Connect()
    {
    }

    [RelayCommand]
    public void Add()
    {
        JoinGroupWindow window = new();
        foreach (var server in Servers)
        {
            window.AddBlackList(server.Name);
        }
        window.ShowDialog();
        window.Close();
        if (window.SelectedServer != null)
        {
            Servers.Add(new(window.SelectedServer, this));
            Save();
        }
        ConnectCommand.NotifyCanExecuteChanged();
    }

    private void Save()
    {
        Group group = _groupService.GetGroup(_id)!;
        group.Servers.Clear();
        foreach (var server in Servers)
        {
            group.Servers.Add(server.Name);
        }
        _groupService.Delete(group.Id);
        _groupService.Add(group);
        Refresh();
    }
}
