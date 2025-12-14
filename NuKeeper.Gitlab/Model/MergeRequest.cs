using System.Text.Json.Serialization;

namespace NuKeeper.Gitlab.Model;

public class MergeRequest
{
    [JsonPropertyName("id")] public string Id { get; set; }

    [JsonPropertyName("title")] public string Title { get; set; }

    [JsonPropertyName("description")] public string Description { get; set; }

    [JsonPropertyName("target_branch")] public string TargetBranch { get; set; }

    [JsonPropertyName("source_branch")] public string SourceBranch { get; set; }

    [JsonPropertyName("remove_source_branch")]
    public bool RemoveSourceBranch { get; set; }

    [JsonPropertyName("labels")] public IList<string> Labels { get; set; }
}
