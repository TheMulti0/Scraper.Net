using System.Text.Json.Serialization;

namespace Scraper.Net.Facebook
{
    public record Error
    {
        [JsonPropertyName("type")]
        public string Type { get; init; }

        [JsonPropertyName("message")]
        public string Message { get; init; }
    }
}