using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace NuKeeper.BitBucketLocal.Models
{
    public class PullRequest
    {
        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; } = string.Empty;

        [JsonPropertyName("state")]
        public string State { get; set; } = "OPEN";

        [JsonPropertyName("open")]
        public bool Open { get; set; } = true;

        [JsonPropertyName("closed")]
        public bool Closed { get; set; } = false;

        [JsonPropertyName("fromRef")]
        public Ref FromRef { get; set; }

        [JsonPropertyName("toRef")]
        public Ref ToRef { get; set; }

        [JsonPropertyName("locked")]
        public bool Locked { get; set; } = false;

        [JsonPropertyName("reviewers")]
        public List<PullRequestReviewer> Reviewers { get; set; }
    }
}


