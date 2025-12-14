using System.Text.Json.Serialization;
using System;

namespace NuKeeper.Gitlab.Model
{
    public class MergeInfo
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("iid")]
        public long Iid { get; set; }

        [JsonPropertyName("project_id")]
        public long ProjectId { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("state")]
        public string State { get; set; }

        [JsonPropertyName("created_at")]
        public DateTime CretedAt { get; set; }

        [JsonPropertyName("updated_at")]
        public DateTime UpdatedAt { get; set; }

        [JsonPropertyName("web_url")]
        public Uri WebUrl { get; set; }

    }
}
