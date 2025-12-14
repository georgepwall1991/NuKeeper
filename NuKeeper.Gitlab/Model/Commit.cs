using System;
using System.Text.Json.Serialization;

namespace NuKeeper.Gitlab.Model
{
    public class Commit
    {
        [JsonPropertyName("author_email")]
        public string AuthorEmail { get; set; }

        [JsonPropertyName("author_name")]
        public string AuthorName { get; set; }

        [JsonPropertyName("authored_date")]
        public DateTimeOffset AuthoredDate { get; set; }

        [JsonPropertyName("committed_date")]
        public DateTimeOffset CommittedDate { get; set; }

        [JsonPropertyName("committer_email")]
        public string CommitterEmail { get; set; }

        [JsonPropertyName("committer_name")]
        public string CommitterName { get; set; }

        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("short_id")]
        public string ShortId { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; }
    }
}
