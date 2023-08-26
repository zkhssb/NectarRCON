using NectarRCON.Models;
using System.Collections.Generic;

namespace NectarRCON.Interfaces;

public interface IRconConfigurationService
{
    void AddInformation(ServerInformation information);
    void Claer();

    IReadOnlyList<ServerInformation> GetInformation();
}
