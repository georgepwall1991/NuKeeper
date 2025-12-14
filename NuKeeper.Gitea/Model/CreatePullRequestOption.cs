using System.Text.Json.Serialization;

namespace NuKeeper.Gitea.Model;

public class CreatePullRequestOption
{
    [JsonPropertyName("assignee")] public string Assignee { get; set; }

    [JsonPropertyName("base")] public string Base { get; set; }

    [JsonPropertyName("body")] public string Body { get; set; }

    [JsonPropertyName("due_date")] public DateTime DueDate { get; set; }

    public string Head { get; set; }

    [JsonPropertyName("milestone")] public long Milestone { get; set; }

    [JsonPropertyName("title")] public string Title { get; set; }

    [JsonPropertyName("assignees")] public IEnumerable<string> Assginees { get; set; }

    [JsonPropertyName("labels")] public IEnumerable<long> Labels { get; set; }
}
