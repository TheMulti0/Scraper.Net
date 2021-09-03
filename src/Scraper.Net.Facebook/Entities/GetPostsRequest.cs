using System.Text.Json.Serialization;

namespace Scraper.Net.Facebook
{
    internal record GetPostsRequest : Request
    {
        [JsonPropertyName("pages")]
        public int Pages { get; init; }
    }
}