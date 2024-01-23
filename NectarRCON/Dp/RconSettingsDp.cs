using System.Text.Json.Serialization;

namespace NectarRCON.Dp;

public class RconSettingsDp:DpFile
{
    protected override string Name => "rcon_settings.json";
    
    /// <summary>
    /// 连接时掉线自动尝试重连
    /// </summary>
    [JsonPropertyName("auto_reconnect")]
    public bool AutoReconnect { get; set; } = true;
    
    /// <summary>
    /// 掉线后不关闭连接窗口
    /// </summary>
    /// <returns></returns>
    [JsonPropertyName("is_keep_connection_window_open")]
    public bool IsKeepConnectionWindowOpen { get; set; } = false;
}