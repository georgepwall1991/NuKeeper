using System.Text.Json.Serialization;

namespace NuKeeper.Gitlab.Model
{
    public class Access
    {
        [JsonPropertyName("access_level")]
        public long AccessLevel { get; set; }

        [JsonPropertyName("notification_level")]
        public long NotificationLevel { get; set; }
    }
}
