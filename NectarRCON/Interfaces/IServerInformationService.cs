using NectarRCON.Models;
using System.Collections.Generic;

namespace NectarRCON.Interfaces;
public interface IServerInformationService
{
    ServerInformation? GetServer(string name);
    List<ServerInformation> GetServers();
    void AddServer(ServerInformation server);
    void Save();
    void RemoveServer(string name);
    void Update(string name, ServerInformation newInfo);
    bool ServerIsExist(string name);
}