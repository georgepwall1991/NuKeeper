using System.Text.Json.Serialization;

namespace NuKeeper.BitBucketLocal.Models;

public class Links
{
    [JsonPropertyName("self")] public List<Link> Self { get; set; }

    [JsonPropertyName("clone")] public List<Link> Clone { get; set; }
}
