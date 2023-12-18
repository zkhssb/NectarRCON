using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace NectarRCON.Updater.Model
{
    public class Release
    {
        [JsonPropertyName("tag_name")]
        public required string TagName { get; set; }

        [JsonPropertyName("name")]
        public required string Name { get; set; }

        [JsonPropertyName("created_at")]
        public required DateTime CreatedAt { get; set; }

        [JsonPropertyName("assets")]
        public required IEnumerable<Asset> Assets { get; set; }

        [JsonPropertyName("body")]
        public required string Body { get; set; }
    }
}
