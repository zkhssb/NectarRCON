using System.Text.Json.Serialization;
namespace NectarRCON.Models;
public class ServerInformation
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
    [JsonPropertyName("address")]
    public string Address { get; set; } = string.Empty;
    [JsonPropertyName("adapter")]
    public string Adapter { get; set; } = string.Empty; // 适配类名
    [JsonPropertyName("port")]
    public ushort Port { get; set; } = 0;
    [JsonIgnore]
    public string DisplayAddress
    {
        get => $"{Address}:{Port}";
    }
}
