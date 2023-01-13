using NectarRCON.Interfaces;
using System.Windows;
using System.Windows.Controls;

namespace NectarRCON.Services;
internal class ConnectingDialogService : IConnectingDialogService
{
    private Grid? _dialog;
    public void Close()
    {
        if (null == _dialog)
            return;
        _dialog.Visibility = Visibility.Hidden;
    }

    public void SetDialog(Grid grid)
    {
        _dialog = grid;
    }

    public void Show()
    {
        if (null == _dialog)
            return;
        _dialog.Visibility = Visibility.Visible;
    }
}
