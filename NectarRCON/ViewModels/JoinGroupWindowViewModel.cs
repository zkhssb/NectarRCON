using CommunityToolkit.Mvvm.ComponentModel;
using NectarRCON.Interfaces;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Data;

namespace NectarRCON.ViewModels;
public partial class JoinGroupWindowViewModel:ObservableObject
{
    private IServerInformationService _serverInformationService;

    [ObservableProperty]
    private ObservableCollection<string> _servers = new();

    [ObservableProperty]
    private ListCollectionView? _serverCollectionView;

    [ObservableProperty]
    public ObservableCollection<string>? _blackList;

    public JoinGroupWindowViewModel()
    {
        _serverInformationService = App.GetService<IServerInformationService>();
        foreach(var server in _serverInformationService.GetServers())
        {
            Servers.Add(server.Name);
        }

        ServerCollectionView = new(Servers);
        ServerCollectionView.Filter += (s) =>
        {
            return !BlackList?.Contains(s?.ToString() ?? string.Empty) ?? true;
        };

        BlackList = new();
    }

    partial void OnBlackListChanged(ObservableCollection<string>? value)
    {
        ServerCollectionView?.Refresh();
    }
}
