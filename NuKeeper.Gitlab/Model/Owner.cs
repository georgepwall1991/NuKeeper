using System.Text.Json.Serialization;

namespace NuKeeper.Gitlab.Model;

public class Owner
{
    [JsonPropertyName("id")] public long Id { get; set; }

    [JsonPropertyName("name")] public string Name { get; set; }

    [JsonPropertyName("created_at")] public DateTimeOffset CreatedAt { get; set; }
}
