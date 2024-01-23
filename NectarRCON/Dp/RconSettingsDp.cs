using System.Text;
using System.Text.Json.Serialization;

namespace NectarRCON.Dp;

public enum RconEncoding
{
    Utf8 = 0,
    Utf16 = 1,
    Utf32 = 2,
    Gb2312 = 3,
    Gbk = 4,
    Gb18030 = 5,
    Ascii = 6,
    Big5 = 7,
    HzGb2312 = 8,
}

public static class RconEncodingExtensions
{
    public static Encoding GetEncoding(this RconEncoding encoding)
        => encoding switch
        {
            RconEncoding.Utf8 => Encoding.UTF8,
            RconEncoding.Utf16 => Encoding.GetEncoding("UTF-16"),
            RconEncoding.Utf32 => Encoding.UTF32,
            RconEncoding.Gb2312 => Encoding.GetEncoding("gb2312"),
            RconEncoding.Gbk => Encoding.GetEncoding("gbk"),
            RconEncoding.Gb18030 => Encoding.GetEncoding("gb18030"),
            RconEncoding.Ascii => Encoding.ASCII,
            RconEncoding.Big5 => Encoding.GetEncoding("big5"),
            RconEncoding.HzGb2312 => Encoding.GetEncoding("hz-gb-2312"),
            _ => Encoding.UTF8,
        };
}

public class RconSettingsDp : DpFile
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
    public bool IsKeepConnectionWindowOpen { get; set; }

    /// <summary>
    /// 文本编码
    /// </summary>
    [JsonPropertyName("encoding")]
    public RconEncoding Encoding { get; set; } = RconEncoding.Utf8;
}