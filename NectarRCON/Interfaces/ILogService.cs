using NectarRCON.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NectarRCON.Interfaces;
public interface ILogService
{
    string GetText();
    string Log(string message);
    void Clear();
    void SetServer(ServerInformation server);
}