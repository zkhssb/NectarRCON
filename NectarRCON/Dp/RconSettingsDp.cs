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
}