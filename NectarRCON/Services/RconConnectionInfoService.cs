using NectarRCON.Interfaces;
using NectarRCON.Models;
using NectarRCON.Rcon;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NectarRCON.Services;

/// <summary>
/// Rcon连接信息管理服务
/// </summary>
public class RconConnectionInfoService : IRconConnectionInfoService
{
    private readonly IServerInformationService _serverInformationService;
    private readonly List<ServerInformation> _serverInformation = new();
    public RconConnectionInfoService(IServerInformationService serverInformationService)
    {
        _serverInformationService = serverInformationService;
    }

    public void AddInformation(string serverName)
        => _serverInformation.Add(_serverInformationService.GetServer(serverName) ?? throw new ArgumentNullException(nameof(serverName)));

    public void Clear()
        => _serverInformation.Clear();

    public IReadOnlyList<ServerInformation> GetInformation()
        => _serverInformation;

    public ServerInformation? GetLastInformation()
        => _serverInformation.FirstOrDefault();
}
