using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace NuKeeper.BitBucketLocal.Models
{
    /// <summary>
    /// Models a page of data.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class IteratorBasedPage<T>
    {
        [JsonPropertyName("size")]
        public ulong? Size { get; set; }

        [JsonPropertyName("limit")]
        public int Limit { get; set; }
        [JsonPropertyName("isLastPage")]
        public bool IsLastPage { get; set; }

        [JsonPropertyName("values")]
        public List<T> Values { get; set; }

        [JsonPropertyName("start")]
        public int Start { get; set; }

        [JsonPropertyName("nextPageStart")]
        public int? NextPageStart { get; set; }
    }
}
