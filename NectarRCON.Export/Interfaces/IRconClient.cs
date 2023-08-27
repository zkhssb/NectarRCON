namespace NectarRCON.Export.Interfaces;
/// <summary>
/// Rcon客户端接口
/// </summary>
public interface IRconClient : IDisposable
{
    /// <summary>
    /// 启动连接
    /// </summary>
    /// <param name="address">
    /// 地址
    /// <code>
    /// localhost:11451
    /// mc.xxxx.net:11451
    /// </code>
    /// </param>
    /// <returns></returns>
    void Start(string address, int port);
    /// <summary>
    /// 关闭连接
    /// </summary>
    void Stop();
    /// <summary>
    /// 执行命令
    /// </summary>
    /// <param name="bytes">原始字节</param>
    /// <returns>服务端返回</returns>
    byte[] Send(byte[] bytes);
    /// <summary>
    /// 是否已连接
    /// </summary>
    bool IsConnected { get; }
}
