using System.Windows.Controls;

namespace NectarRCON.Interfaces;
public interface IConnectingDialogService
{
    void SetDialog(Grid grid);
    void Show();
    void Close();
}