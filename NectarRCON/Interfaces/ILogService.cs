using NectarRCON.Models;

namespace NectarRCON.Interfaces;
public interface ILogService
{
    string GetText();
    string Log(string message);
    void Clear();
    void SetServer(ServerInformation server);
}