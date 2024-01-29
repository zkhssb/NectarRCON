using System.Text.Json.Serialization;
namespace NectarRCON.Models;
public enum RconAdapter
{
    Minecraft,
    Palworld
}

public static class RconAdapterExtensions
{
    public static string ToAdapterString(this RconAdapter adapter)
    {
        return adapter switch
        {
            RconAdapter.Minecraft => "rcon.minecraft",
            RconAdapter.Palworld => "rcon.palworld",
            _ => string.Empty
        };
    }
    
    public static RconAdapter ToAdapter(this string adapter)
    {
        return adapter switch
        {
            "rcon.minecraft" => RconAdapter.Minecraft,
            "rcon.palworld" => RconAdapter.Palworld,
            _ => RconAdapter.Minecraft
        };
    }
}

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
