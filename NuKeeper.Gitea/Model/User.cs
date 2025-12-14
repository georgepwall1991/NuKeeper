using System.Text.Json.Serialization;

namespace NuKeeper.Gitea.Model
{
    public class User
    {
        [JsonPropertyName("avatur_url")]
        public string AvatarUrl { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("full_name")]
        public string FullName { get; set; }

        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("is_admin")]
        public bool IsAdmin { get; set; }

        [JsonPropertyName("language")]
        public string Language { get; set; }

        [JsonPropertyName("login")]
        public string Login { get; set; }
    }
}
