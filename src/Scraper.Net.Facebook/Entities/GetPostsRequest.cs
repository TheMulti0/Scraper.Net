using System.Text.Json.Serialization;

namespace Scraper.Net.Facebook
{
    internal record GetPostsRequest : Request
    {
        [JsonPropertyName("pages")]
        public int Pages { get; init; }

        [JsonPropertyName("posts_per_page")]
        public int PostsPerPage { get; init; }
    }
}