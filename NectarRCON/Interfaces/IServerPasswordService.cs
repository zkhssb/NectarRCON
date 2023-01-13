using NectarRCON.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NectarRCON.Interfaces;
public interface IServerPasswordService
{
    void Save();
    bool IsExist(ServerInformation server);
    void Set(ServerInformation server, string? password, bool? isEmpty);
    void Remove(ServerInformation server);
    ServerPassword? Get(ServerInformation server);
    ServerInformation GetSelect();
    void Select(ServerInformation server);
}
