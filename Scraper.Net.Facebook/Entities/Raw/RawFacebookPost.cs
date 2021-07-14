using System;
using System.Text.Json.Serialization;

namespace Scraper.Net.Facebook.Entities.Raw
{
    internal record RawFacebookPost 
    {
        [JsonPropertyName("available")]
        public bool Available { get; init; }
        
        [JsonPropertyName("post_id")]
        public string PostId { get; init; }

        [JsonPropertyName("text")]
        public string Text { get; init; }
        
        [JsonPropertyName("post_text")]
        public string PostText { get; init; }
        
        [JsonPropertyName("time")]
        [JsonConverter(typeof(DateTimeConverterUsingDateTimeParse))]
        public DateTime? Time { get; init; }
        
        [JsonPropertyName("post_url")]
        public string PostUrl { get; init; }
        
        [JsonPropertyName("is_live")]
        public bool IsLive { get; init; }
        
        [JsonPropertyName("link")]
        public string Link { get; init; }

        #region Image

        [JsonPropertyName("image")]
        public string Image { get; init; }
        
        [JsonPropertyName("image_id")]
        public string ImageId { get; init; }
        
        [JsonPropertyName("image_ids")]
        public string[] ImageIds { get; init; }
        
        [JsonPropertyName("image_lowquality")]
        public string ImageLowQuality { get; init; }
        
        [JsonPropertyName("images")]
        public string[] Images { get; init; }
        
        [JsonPropertyName("images_description")]
        public string[] ImagesDescription { get; init; }
        
        [JsonPropertyName("images_lowquality")]
        public string[] ImagesLowQuality { get; init; }
        
        [JsonPropertyName("images_lowquality_description")]
        public string[] ImagesLowQualityDescription { get; init; }

        #endregion

        #region Video

        [JsonPropertyName("video")]
        public string VideoUrl { get; init; }

        [JsonPropertyName("video_duration_seconds")]
        public int? VideoDurationSeconds { get; init; }
        
        [JsonPropertyName("video_width")]
        public int? VideoWidth { get; init; }
        
        [JsonPropertyName("video_height")]
        public int? VideoHeight { get; init; }

        [JsonPropertyName("video_quality")]
        public string VideoQuality { get; init; }
        
        [JsonPropertyName("video_thumbnail")]
        public string VideoThumbnail { get; init; }
        
        [JsonPropertyName("video_id")]
        public string VideoId { get; init; }

        [JsonPropertyName("video_size_MB")]
        public double? VideoSizeMb { get; init; }
        
        [JsonPropertyName("video_watches")]
        public int? VideoWatches { get; init; }

        #endregion

        #region Author

        [JsonPropertyName("user_id")]
        public string UserId { get; init; }

        [JsonPropertyName("user_url")]
        public string UserUrl { get; init; }
        
        [JsonPropertyName("username")]
        public string UserName { get; init; }

        #endregion

        #region Stats

        [JsonPropertyName("comments")]
        public int Comments { get; init; }
        
        [JsonPropertyName("shares")]
        public int Shares { get; init; }
        
        [JsonPropertyName("likes")]
        public int Likes { get; init; }

        #endregion

        #region Shared

        [JsonPropertyName("shared_post_id")]
        public string SharedPostId { get; init; }
        
        [JsonPropertyName("shared_post_url")]
        public string SharedPostUrl { get; init; }
        
        [JsonPropertyName("shared_text")]
        public string SharedText { get; init; }
        
        [JsonPropertyName("shared_time")]
        public DateTime? SharedTime { get; init; }

        [JsonPropertyName("shared_user_id")]
        public string SharedUserId { get; init; }

        [JsonPropertyName("shared_username")]
        public string SharedUserName { get; init; }

        #endregion
        
        #region Unknown

        [JsonPropertyName("factcheck")]
        public object FactCheck { get; set; }

        [JsonPropertyName("reactions")]
        public object Reactions { get; set; }

        [JsonPropertyName("reactors")]
        public object Reactors { get; set; }

        #endregion

        [JsonPropertyName("comments_full")]
        public RootComment[] CommentsFull { get; init; }
    }
}