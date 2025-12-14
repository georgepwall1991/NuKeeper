using System.Text.Json.Serialization;

namespace NuKeeper.Gitea.Model;

public class Repository
{
    [JsonPropertyName("archived")] public bool IsArchived { get; set; }

    [JsonPropertyName("clone_url")] public string CloneUrl { get; set; }

    [JsonPropertyName("created_at")] public DateTime CreateDate { get; set; }

    [JsonPropertyName("default_branch")] public string DefaultBranch { get; set; }

    [JsonPropertyName("name")] public string Name { get; set; }

    [JsonPropertyName("fork")] public bool IsFork { get; set; }

    [JsonPropertyName("id")] public int Id { get; set; }

    [JsonPropertyName("owner")] public Owner Owner { get; set; }

    [JsonPropertyName("full_name")] public string FullName { get; set; }

    [JsonPropertyName("description")] public string Description { get; set; }

    [JsonPropertyName("empty")] public bool IsEmpty { get; set; }

    [JsonPropertyName("parent")] public Repository Parent { get; set; }

    [JsonPropertyName("mirror")] public bool IsMirror { get; set; }

    [JsonPropertyName("size")] public int Size { get; set; }

    [JsonPropertyName("html_url")] public string HtmlUrl { get; set; }

    [JsonPropertyName("ssh_url")] public string SshUrl { get; set; }

    [JsonPropertyName("website")] public string Website { get; set; }

    [JsonPropertyName("stars_count")] public int StarsCount { get; set; }

    [JsonPropertyName("forks_count")] public int ForksCount { get; set; }

    [JsonPropertyName("watchers_count")] public int WatchersCount { get; set; }

    [JsonPropertyName("open_issues_count")]
    public int OpenIssuesCount { get; set; }

    [JsonPropertyName("permissions")] public Permissions Permissions { get; set; }
}
