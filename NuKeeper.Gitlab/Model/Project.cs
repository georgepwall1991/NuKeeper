using System;
using System.Text.Json.Serialization;

namespace NuKeeper.Gitlab.Model
{
    public class Project
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("description")]
        public object Description { get; set; }

        [JsonPropertyName("default_branch")]
        public string DefaultBranch { get; set; }

        [JsonPropertyName("visibility")]
        public string Visibility { get; set; }

        [JsonPropertyName("ssh_url_to_repo")]
        public string SshUrlToRepo { get; set; }

        [JsonPropertyName("http_url_to_repo")]
        public Uri HttpUrlToRepo { get; set; }

        [JsonPropertyName("web_url")]
        public Uri WebUrl { get; set; }

        [JsonPropertyName("readme_url")]
        public Uri ReadmeUrl { get; set; }

        [JsonPropertyName("owner")]
        public Owner Owner { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("name_with_namespace")]
        public string NameWithNamespace { get; set; }

        [JsonPropertyName("path")]
        public string Path { get; set; }

        [JsonPropertyName("path_with_namespace")]
        public string PathWithNamespace { get; set; }

        [JsonPropertyName("issues_enabled")]
        public bool IssuesEnabled { get; set; }

        [JsonPropertyName("open_issues_count")]
        public long OpenIssuesCount { get; set; }

        [JsonPropertyName("merge_requests_enabled")]
        public bool MergeRequestsEnabled { get; set; }

        [JsonPropertyName("jobs_enabled")]
        public bool JobsEnabled { get; set; }

        [JsonPropertyName("wiki_enabled")]
        public bool WikiEnabled { get; set; }

        [JsonPropertyName("snippets_enabled")]
        public bool SnippetsEnabled { get; set; }

        [JsonPropertyName("resolve_outdated_diff_discussions")]
        public bool ResolveOutdatedDiffDiscussions { get; set; }

        [JsonPropertyName("container_registry_enabled")]
        public bool ContainerRegistryEnabled { get; set; }

        [JsonPropertyName("created_at")]
        public DateTimeOffset CreatedAt { get; set; }

        [JsonPropertyName("last_activity_at")]
        public DateTimeOffset LastActivityAt { get; set; }

        [JsonPropertyName("creator_id")]
        public long CreatorId { get; set; }

        [JsonPropertyName("import_status")]
        public string ImportStatus { get; set; }

        [JsonPropertyName("archived")]
        public bool Archived { get; set; }

        [JsonPropertyName("avatar_url")]
        public Uri AvatarUrl { get; set; }

        [JsonPropertyName("shared_runners_enabled")]
        public bool SharedRunnersEnabled { get; set; }

        [JsonPropertyName("forks_count")]
        public long ForksCount { get; set; }

        [JsonPropertyName("star_count")]
        public long StarCount { get; set; }

        [JsonPropertyName("runners_token")]
        public string RunnersToken { get; set; }

        [JsonPropertyName("public_jobs")]
        public bool PublicJobs { get; set; }

        [JsonPropertyName("only_allow_merge_if_pipeline_succeeds")]
        public bool OnlyAllowMergeIfPipelineSucceeds { get; set; }

        [JsonPropertyName("only_allow_merge_if_all_discussions_are_resolved")]
        public bool OnlyAllowMergeIfAllDiscussionsAreResolved { get; set; }

        [JsonPropertyName("request_access_enabled")]
        public bool RequestAccessEnabled { get; set; }

        [JsonPropertyName("merge_method")]
        public string MergeMethod { get; set; }

        [JsonPropertyName("statistics")]
        public Statistics Statistics { get; set; }

        [JsonPropertyName("_links")]
        public Links Links { get; set; }

        [JsonPropertyName("import_error")]
        public object ImportError { get; set; }

        [JsonPropertyName("permissions")]
        public Permissions Permissions { get; set; }

        [JsonPropertyName("forked_from_project")]
        public ForkedFromProject ForkedFromProject { get; set; }
    }
}
