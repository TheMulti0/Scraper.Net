using System.Text.Json.Serialization;

namespace Scraper.Net.Facebook.Scraper
{
    public record RootComment : Comment
    {
        [JsonPropertyName("replies")]
        public Comment[] Replies { get; init; }
    }
}