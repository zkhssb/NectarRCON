using System.Text.Json.Serialization;

namespace NectarRCON.Updater.Model
{
    public class Asset
    {
        [JsonPropertyName("name")]
        public required string Name { get; set; }
        [JsonPropertyName("url")]
        public required string Url { get; set; }
        [JsonPropertyName("created_at")]
        public required DateTime CreatedAt { get; set; }
    }
}
