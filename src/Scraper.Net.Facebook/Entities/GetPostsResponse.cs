using System.Text.Json.Serialization;

namespace Scraper.Net.Facebook
{
    internal record GetPostsResponse
    {
        [JsonPropertyName("posts")]
        public RawFacebookPost[] Posts { get; init; }

        [JsonPropertyName("error")]
        public string Error { get; init; }
        
        [JsonPropertyName("error_description")]
        public string ErrorDescription { get; init; }

        [JsonIgnore]
        internal GetPostsRequest OriginalRequest { get; init; }
    }
}