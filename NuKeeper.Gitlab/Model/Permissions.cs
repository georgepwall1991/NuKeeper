using System.Text.Json.Serialization;

namespace NuKeeper.Gitlab.Model;

public class Permissions
{
    [JsonPropertyName("project_access")] public Access ProjectAccess { get; set; }

    [JsonPropertyName("group_access")] public Access GroupAccess { get; set; }
}
