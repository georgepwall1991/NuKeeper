using System.Text.Json.Serialization;

namespace NuKeeper.Gitlab.Model;

public class ForkedFromProject
{
    [JsonPropertyName("id")] public long Id { get; set; }

    [JsonPropertyName("description")] public string Description { get; set; }

    [JsonPropertyName("name")] public string Name { get; set; }

    [JsonPropertyName("name_with_namespace")]
    public string NameWithNamespace { get; set; }

    [JsonPropertyName("path")] public string Path { get; set; }

    [JsonPropertyName("path_with_namespace")]
    public string PathWithNamespace { get; set; }

    [JsonPropertyName("created_at")] public DateTimeOffset CreatedAt { get; set; }

    [JsonPropertyName("default_branch")] public string DefaultBranch { get; set; }

    [JsonPropertyName("ssh_url_to_repo")] public string SshUrlToRepo { get; set; }

    [JsonPropertyName("http_url_to_repo")] public Uri HttpUrlToRepo { get; set; }

    [JsonPropertyName("web_url")] public Uri WebUrl { get; set; }

    [JsonPropertyName("avatar_url")] public Uri AvatarUrl { get; set; }

    [JsonPropertyName("license_url")] public Uri LicenseUrl { get; set; }

    [JsonPropertyName("star_count")] public long StarCount { get; set; }

    [JsonPropertyName("forks_count")] public long ForksCount { get; set; }

    [JsonPropertyName("last_activity_at")] public DateTimeOffset LastActivityAt { get; set; }
}
