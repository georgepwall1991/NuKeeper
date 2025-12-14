using System.Text.Json.Serialization;

namespace NuKeeper.BitBucketLocal.Models
{
    public class Project
    {
        [JsonPropertyName("key")]
        public string Key { get; set; }

        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("public")]
        public bool Public { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("links")]
        public Links Links { get; set; }
    }
}
