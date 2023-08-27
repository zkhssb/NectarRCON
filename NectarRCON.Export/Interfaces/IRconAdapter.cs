namespace NectarRCON.Export.Interfaces;
/// <summary>
/// Rcon协议兼容接口
/// </summary>
public interface IRconAdapter : IDisposable
{
    /// <summary>
    /// 已经连接
    /// </summary>
    bool IsConnected { get; }

    /// <summary>
    /// 连接
    /// </summary>
    bool Connect(string address, int port);


    /// <summary>
    /// 验证
    /// </summary>
    bool Authenticate(string password);

    /// <summary>
    /// 断开连接
    /// </summary>
    void Disconnect();

    /// <summary>
    /// 执行命令
    /// </summary>

    string Run(string command);
}
