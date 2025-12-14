using System.Text.Json.Serialization;

namespace NuKeeper.Gitea.Model;

public class BranchInfo
{
    [JsonPropertyName("commit")] public Commit Commit { get; set; }

    [JsonPropertyName("name")] public string Name { get; set; }
}
