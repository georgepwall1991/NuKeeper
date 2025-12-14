using System.Text.Json.Serialization;

namespace NuKeeper.BitBucketLocal.Models;

public class PullRequestReviewer
{
    [JsonPropertyName("user")] public Reviewer User { get; set; }
}
