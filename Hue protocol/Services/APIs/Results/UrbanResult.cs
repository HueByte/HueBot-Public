using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace HueProtocol.Services.APIs.Results
{
    public struct UrbanResult
    {
        [JsonPropertyName("list")]
        public IReadOnlyList<UrbanDefinition> Definitions { get; set; }
    }

    public struct UrbanDefinition
    {
        [JsonPropertyName("definition")]
        public string Definition { get; set; }

        [JsonPropertyName("word")]
        public string Word { get; set; }

        [JsonPropertyName("author")]
        public string Author { get; set; }

        [JsonPropertyName("example")]
        public string Example { get; set; }
    }
}