using System.Text.Json.Serialization;

namespace NuKeeper.BitBucketLocal.Models;

public class Conditions
{
    [JsonPropertyName("id")] public int Id { get; set; }

    [JsonPropertyName("reviewers")] public List<Reviewer> Reviewers { get; set; }
}
