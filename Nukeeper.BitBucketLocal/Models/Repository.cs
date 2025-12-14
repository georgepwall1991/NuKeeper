using System.Text.Json.Serialization;

namespace NuKeeper.BitBucketLocal.Models
{
    public class Repository
    {
        [JsonPropertyName("slug")]
        public string Slug { get; set; }

        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("scmId")]
        public string ScmId { get; set; }

        [JsonPropertyName("state")]
        public string State { get; set; }

        public bool? Forkable { get; set; }

        [JsonPropertyName("project")]
        public Project Project { get; set; }

        [JsonPropertyName("public")]
        public bool? Public { get; set; }

        [JsonPropertyName("links")]
        public Links Links { get; set; }
    }
}

