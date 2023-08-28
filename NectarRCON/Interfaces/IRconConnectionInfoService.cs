using NectarRCON.Models;
using System.Collections.Generic;

namespace NectarRCON.Interfaces;
public interface IRconConnectionInfoService
{
    /// <summary>
    /// 清除所有连接信息
    /// </summary>
    void Clear();

    /// <summary>
    /// 添加一个服务器
    /// </summary>
    void AddInformation(string serverName);

    /// <summary>
    /// 获取最后一个被添加的服务器
    /// </summary>
    ServerInformation? GetLastInformation();

    /// <summary>
    /// 获取所有的信息
    /// </summary>
    IReadOnlyList<ServerInformation> GetInformation();

    /// <summary>
    /// 是否拥有多个服务器信息
    /// </summary>
    public bool HasMultipleInformation
        => GetInformation().Count > 1;
}
