using System.Text.Json.Serialization;

namespace NuKeeper.Gitlab.Model;

public class User
{
    [JsonPropertyName("id")] public int Id { get; set; }

    [JsonPropertyName("username")] public string UserName { get; set; }

    [JsonPropertyName("name")] public string Name { get; set; }

    [JsonPropertyName("email")] public string Email { get; set; }
}
