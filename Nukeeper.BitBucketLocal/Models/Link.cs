using System.Text.Json.Serialization;

namespace NuKeeper.BitBucketLocal.Models;

public class Link
{
    [JsonPropertyName("href")] public string Href { get; set; }

    [JsonPropertyName("name")] public string Name { get; set; }
}
