using System.Text.Json.Serialization;

namespace NuKeeper.Gitea.Model;

public class Actor
{
    [JsonPropertyName("email")] public string Email { get; set; }

    [JsonPropertyName("name")] public string Name { get; set; }

    [JsonPropertyName("username")] public string UserName { get; set; }
}
