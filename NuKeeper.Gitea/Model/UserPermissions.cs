using System.Text.Json.Serialization;

namespace NuKeeper.Gitea.Model
{
    public class Permissions
    {
        [JsonPropertyName("admin")]
        public bool IsAdmin { get; set; }

        [JsonPropertyName("pull")]
        public bool IsPull { get; set; }

        [JsonPropertyName("push")]
        public bool IsPush { get; set; }
    }
}
