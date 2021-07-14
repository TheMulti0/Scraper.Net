using System;
using System.Text.Json.Serialization;

namespace Scraper.Net.Facebook.Entities
{
    public record Comment
    {
        [JsonPropertyName("comment_id")]
        public string CommentId { get; init; }

        [JsonPropertyName("comment_image")]
        public string CommentImageUrl { get; init; }

        [JsonPropertyName("comment_text")]
        public string CommentText { get; init; }
        
        [JsonPropertyName("comment_time")]
        public TimeSpan CommentTime { get; init; }

        [JsonPropertyName("comment_url")]
        public string CommentUrl { get; init; }

        [JsonPropertyName("commenter_id")]
        public string CommenterId { get; init; }

        [JsonPropertyName("commenter_meta")]
        public object CommenterMeta { get; init; }

        [JsonPropertyName("commenter_name")]
        public string CommenterName { get; init; }

        [JsonPropertyName("commenter_url")]
        public string CommenterUrl { get; init; }
    }
}