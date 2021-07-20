using System.Text.Json.Serialization;

namespace Scraper.Net.Facebook
{
    internal record RootComment : Comment
    {
        [JsonPropertyName("replies")]
        public Comment[] Replies { get; init; }
    }
}