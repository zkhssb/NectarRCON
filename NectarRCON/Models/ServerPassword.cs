using System.Text.Json.Serialization;

namespace NectarRCON.Models;
public class ServerPassword
{
    [JsonPropertyName("password")]
    public string Password { get; set; } = string.Empty;
    [JsonPropertyName("is_empty")]
    public bool IsEmpty { get; set; }
    [JsonPropertyName("name")]
    public string ServerName { get; set; } = string.Empty;
}