using System.Text.Json.Serialization;

namespace NuKeeper.Gitea.Model;

public class PullRequest
{
    [JsonPropertyName("assignee")] public User Assignee { get; set; }

    [JsonPropertyName("assignees")] public IEnumerable<User> Assignees { get; set; }

    [JsonPropertyName("base")] public PullRequestBranchInfo Base { get; set; }

    [JsonPropertyName("title")] public string Title { get; set; }

    [JsonPropertyName("body")] public string Body { get; set; }

    [JsonPropertyName("closed_at")] public DateTime? CloseDate { get; set; }

    [JsonPropertyName("created_at")] public DateTime? CreateDate { get; set; }

    [JsonPropertyName("due_date")] public DateTime? DueDate { get; set; }

    [JsonPropertyName("comments")] public long Comments { get; set; }

    [JsonPropertyName("diff_url")] public string DiffUrl { get; set; }

    [JsonPropertyName("head")] public PullRequestBranchInfo Head { get; set; }

    [JsonPropertyName("html_url")] public string HtmlUrl { get; set; }

    [JsonPropertyName("id")] public long Id { get; set; }

    [JsonPropertyName("labels")] public IEnumerable<Label> Labels { get; set; }

    [JsonPropertyName("merge_base")] public string MergeBase { get; set; }

    [JsonPropertyName("merge_commit_sha")] public string MergeCommitSha { get; set; }

    [JsonPropertyName("mergeable")] public bool IsMergeable { get; set; }

    [JsonPropertyName("merged")] public bool IsMerged { get; set; }

    [JsonPropertyName("merged_at")] public DateTime? MergedAt { get; set; }

    [JsonPropertyName("merged_by")] public User MergedBy { get; set; }

    [JsonPropertyName("number")] public long Number { get; set; }

    [JsonPropertyName("patch_url")] public string PatchUrl { get; set; }

    [JsonPropertyName("updated_at")] public DateTime? UpdatedAt { get; set; }

    [JsonPropertyName("url")] public string Url { get; set; }

    [JsonPropertyName("user")] public User User { get; set; }
}
