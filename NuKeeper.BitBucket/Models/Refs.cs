using System.Text.Json.Serialization;

namespace NuKeeper.BitBucket.Models;

public class Refs
{
    [JsonPropertyName("pagelen")] public long Pagelen { get; set; }

    [JsonPropertyName("values")] public List<Ref> Values { get; set; }

    [JsonPropertyName("page")] public long Page { get; set; }

    [JsonPropertyName("next")] public Uri Next { get; set; }
}

public class Ref
{
    [JsonPropertyName("name")] public string Name { get; set; }

    [JsonPropertyName("links")] public ValueLinks Links { get; set; }

    [JsonPropertyName("default_merge_strategy")]
    public string DefaultMergeStrategy { get; set; }

    [JsonPropertyName("merge_strategies")] public List<string> MergeStrategies { get; set; }

    [JsonPropertyName("type")] public string Type { get; set; }

    [JsonPropertyName("target")] public Target Target { get; set; }
}

public class ValueLinks
{
    [JsonPropertyName("commits")] public Commits Commits { get; set; }

    [JsonPropertyName("self")] public Commits Self { get; set; }

    [JsonPropertyName("html")] public Commits Html { get; set; }
}

public class Commits
{
    [JsonPropertyName("href")] public Uri Href { get; set; }
}

public class Target
{
    [JsonPropertyName("hash")] public string Hash { get; set; }

    [JsonPropertyName("repository")] public Repository Repository { get; set; }

    [JsonPropertyName("links")] public TargetLinks Links { get; set; }

    [JsonPropertyName("author")] public Author Author { get; set; }

    [JsonPropertyName("parents")] public List<Parent> Parents { get; set; }

    [JsonPropertyName("date")] public DateTimeOffset Date { get; set; }

    [JsonPropertyName("message")] public string Message { get; set; }

    [JsonPropertyName("type")] public string Type { get; set; }
}

public class UserLinks
{
    [JsonPropertyName("self")] public Commits Self { get; set; }

    [JsonPropertyName("html")] public Commits Html { get; set; }

    [JsonPropertyName("avatar")] public Commits Avatar { get; set; }
}

public class TargetLinks
{
    [JsonPropertyName("self")] public Commits Self { get; set; }

    [JsonPropertyName("comments")] public Commits Comments { get; set; }

    [JsonPropertyName("patch")] public Commits Patch { get; set; }

    [JsonPropertyName("html")] public Commits Html { get; set; }

    [JsonPropertyName("diff")] public Commits Diff { get; set; }

    [JsonPropertyName("approve")] public Commits Approve { get; set; }

    [JsonPropertyName("statuses")] public Commits Statuses { get; set; }
}
