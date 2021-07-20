using System.Text.Json.Serialization;

namespace Scraper.Net.Facebook
{
    public record PageInfo
    {
        [JsonPropertyName("image")]
        public string Image { get; init; }

        [JsonPropertyName("name")]
        public string Name { get; init; }
        
        [JsonPropertyName("about")]
        public string About { get; init; }

        [JsonPropertyName("type")]
        public string Type { get; init; }

        [JsonPropertyName("url")]
        public string Url { get; init; }

        [JsonPropertyName("followers")]
        public int Followers { get; init; }
        
        [JsonPropertyName("likes")]
        public int Likes { get; init; }
    }
}