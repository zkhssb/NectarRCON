using System.Text.Json.Serialization;

namespace NectarRCON.Models;
public partial class Config
{
    [JsonPropertyName("language")]
    public string LanguageName { get; set; } = string.Empty;
    [JsonPropertyName("theme")]
    public ETheme Theme { get; set; }
    [JsonPropertyName("command_list")]
    public int CommandLimit;
}