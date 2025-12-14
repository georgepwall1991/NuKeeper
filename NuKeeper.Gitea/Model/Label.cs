using System.Text.Json.Serialization;

namespace NuKeeper.Gitea.Model;

public class Label
{
    [JsonPropertyName("color")] public string Color { get; set; }

    [JsonPropertyName("id")] public long Id { get; set; }

    [JsonPropertyName("name")] public string Name { get; set; }

    [JsonPropertyName("url")] public string Url { get; set; }
}
