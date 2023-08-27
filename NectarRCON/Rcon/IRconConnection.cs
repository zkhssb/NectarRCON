using NectarRCON.Models;
using System.Threading.Tasks;

namespace NectarRCON.Rcon;
public delegate void MessageEvent(ServerInformation information, string message);
public delegate void RconEvent(ServerInformation information);
public interface IRconConnection
{
    event MessageEvent OnMessage;
    event RconEvent OnClosed;
    event RconEvent OnConnected;
    event RconEvent OnConnecting;
    void Connect();
    void Send(string command);
    void Close();
    bool IsConnected();
    bool IsConnecting();
}