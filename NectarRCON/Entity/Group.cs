using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace NectarRCON.Models
{
    public class Group
    {
        [JsonPropertyName("name")]
        public required string Name { get; set; }
        [JsonPropertyName("server_list")]
        public required List<string> Servers { get; set; }
    }
}
