using System.Text.Json.Serialization;

namespace NuKeeper.Gitlab.Model;

public class Links
{
    [JsonPropertyName("self")] public Uri Self { get; set; }

    [JsonPropertyName("issues")] public Uri Issues { get; set; }

    [JsonPropertyName("merge_requests")] public Uri MergeRequests { get; set; }

    [JsonPropertyName("repo_branches")] public Uri RepoBranches { get; set; }

    [JsonPropertyName("labels")] public Uri Labels { get; set; }

    [JsonPropertyName("events")] public Uri Events { get; set; }

    [JsonPropertyName("members")] public Uri Members { get; set; }
}
