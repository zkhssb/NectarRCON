using NectarRCON.Models;
using System.Threading.Tasks;

namespace NectarRCON.Interfaces;
public delegate void MessageEvent(ServerInformation information, string message);
public delegate void RconEvent(ServerInformation information);
public interface IRconConnection
{
    event MessageEvent OnMessage;
    event RconEvent OnClosed;
    event RconEvent OnConnected;
    event RconEvent OnConnecting;
    Task ConnectAsync(ServerInformation info);
    Task Send(string command);
    void Close();
    bool IsConnected();
    bool IsConnecting();
}