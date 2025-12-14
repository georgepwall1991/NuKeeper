using System.Text.Json.Serialization;

namespace NuKeeper.BitBucketLocal.Models
{
    public class Branch
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("displayId")]
        public string DisplayId { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("latestCommit")]
        public string LatestCommit { get; set; }

        [JsonPropertyName("latestChangeset")]
        public string LatestChangeset { get; set; }

        [JsonPropertyName("isDefault")]
        public bool IsDefault { get; set; }
    }
}
