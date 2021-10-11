using System.Text.Json.Serialization;

namespace Scraper.Net.Facebook
{
    internal record Link
    {
        [JsonPropertyName("link")]
        public string Url { get; init; }
        
        [JsonPropertyName("text")]
        public string Text { get; init; }
    }
}