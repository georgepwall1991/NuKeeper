using System.Text.Json.Serialization;

namespace NuKeeper.Gitea.Model
{
    public class PullRequestBranchInfo
    {
        [JsonPropertyName("label")]
        public string Label { get; set; }

        [JsonPropertyName("ref")]
        public string Ref { get; set; }

        [JsonPropertyName("repo")]
        public Repository Repo { get; set; }

        [JsonPropertyName("repo_id")]
        public long RepoId { get; set; }

        [JsonPropertyName("sha")]
        public string Sha { get; set; }
    }
}
