using System.Text.Json.Serialization;

namespace Scraper.Net.Facebook.Entities.Raw
{
    public record RootComment : Comment
    {
        [JsonPropertyName("replies")]
        public Comment[] Replies { get; init; }
    }
}