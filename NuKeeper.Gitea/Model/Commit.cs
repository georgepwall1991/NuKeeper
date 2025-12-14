using System.Text.Json.Serialization;

namespace NuKeeper.Gitea.Model;

public class Commit
{
    [JsonPropertyName("author")] public Actor Author { get; set; }

    [JsonPropertyName("comitter")] public Actor Comitter { get; set; }

    [JsonPropertyName("timestamp")] public DateTime AuthoredDate { get; set; }

    [JsonPropertyName("id")] public string Id { get; set; }

    [JsonPropertyName("short_id")] public string ShortId { get; set; }

    [JsonPropertyName("title")] public string Title { get; set; }

    [JsonPropertyName("url")] public string Url { get; set; }
}
