using System.Text.Json.Serialization;

namespace Scraper.Net.Facebook
{
    public record RootComment : Comment
    {
        [JsonPropertyName("replies")]
        public Comment[] Replies { get; init; }
    }
}