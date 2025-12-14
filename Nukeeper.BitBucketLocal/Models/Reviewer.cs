using System.Text.Json.Serialization;

namespace NuKeeper.BitBucketLocal.Models
{
    public class Reviewer
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("active")]
        public bool Active { get; set; }
    }
}
