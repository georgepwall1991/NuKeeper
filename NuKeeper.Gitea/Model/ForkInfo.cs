using System.Text.Json.Serialization;

namespace NuKeeper.Gitea.Model
{
    public class ForkInfo
    {
        public ForkInfo(string organizationName)
        {
            Organization = organizationName;
        }

        [JsonPropertyName("organization")]
        public string Organization { get; set; }
    }
}
