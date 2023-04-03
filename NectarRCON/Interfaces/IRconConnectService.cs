using NectarRCON.Models;
using System.Threading.Tasks;

namespace NectarRCON.Interfaces;
public delegate void MessageEvent(ServerInformation information, string message);
public delegate void EventEvent(ServerInformation information);
public delegate void ClosedEvent(ServerInformation information);
public interface IRconConnectService
{
    event MessageEvent OnMessage;
    event ClosedEvent OnClosed;
    event EventEvent OnConnected;
    event EventEvent OnConnecting;
    Task ConnectAsync(ServerInformation info);
    Task Send(string command);
    void Close();
    bool IsConnected();
    bool IsConnecting();
}