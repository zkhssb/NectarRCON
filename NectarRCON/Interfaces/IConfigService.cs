using NectarRCON.Models;

namespace NectarRCON.Interfaces;
public interface IConfigService
{
    void Save();
    Config GetConfig();
}