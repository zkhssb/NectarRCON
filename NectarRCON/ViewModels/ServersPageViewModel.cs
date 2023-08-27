using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NectarRCON.Interfaces;
using NectarRCON.Models;
using NectarRCON.Windows;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Data;

namespace NectarRCON.ViewModels;
public partial class ServersPageViewModel : ObservableObject
{
    [ObservableProperty]
    private ListCollectionView _serverCollectionView;

    [ObservableProperty]
    private ObservableCollection<ServerModel> _servers;

    private string _filterName = string.Empty;
#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。
    public ServersPageViewModel()
#pragma warning restore CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。
    {
        Servers = new();
        ServerCollectionView = new(Servers);

        ServerCollectionView.Filter += (s) =>
        {
            if (_filterName == string.Empty)
                return true;
            if (s is ServerModel model)
            {
                return model.Name.Contains(_filterName);
            }
            return false;
        };

        Refresh();
    }

    public void Refresh()
    {
        Servers.Clear();
        foreach (ServerInformation information in App.GetService<IServerInformationService>().GetServers())
        {
            Servers.Add(new ServerModel(information, this));
        }
    }

    [RelayCommand]
    public void AddServer()
    {
        AddServerWindow addServer = App.GetService<AddServerWindow>();
        addServer.ShowDialog();
        Refresh();
    }
    
    [RelayCommand]
    public void FilterTextChanged(TextChangedEventArgs e)
    {
        var box = (System.Windows.Controls.TextBox)e.Source;
        _filterName = box.Text.ToString() ?? string.Empty;
        _serverCollectionView.Refresh();
    }
}
