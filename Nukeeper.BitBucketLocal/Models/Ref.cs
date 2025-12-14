using System.Text.Json.Serialization;

namespace NuKeeper.BitBucketLocal.Models;

public class Ref
{
    [JsonPropertyName("id")] public string Id { get; set; }

    [JsonPropertyName("repository")] public Repository Repository { get; set; }
}
