using System;
using System.Text.Json.Serialization;

namespace Scraper.Net
{
    public record VideoItem : IMediaItem
    {
        public string Url { get; init; }
        public UrlType UrlType { get; init; }
        public string ThumbnailUrl { get; init; }
        [JsonConverter(typeof(NullableTimeSpanConverter))]
        public TimeSpan? Duration { get; init; }
        public int? Width { get; init; }
        public int? Height { get; init; }

        public VideoItem()
        {
        }

        [JsonConstructor]
        public VideoItem(
            string url,
            UrlType urlType,
            string thumbnailUrl,
            TimeSpan? duration = null,
            int? width = null,
            int? height = null)
        {
            Url = url;
            UrlType = urlType;
            ThumbnailUrl = thumbnailUrl;
            Duration = duration;
            Width = width;
            Height = height;
        }
        
        public VideoItem(
            string url,
            UrlType urlType,
            string thumbnailUrl,
            double? durationSeconds = null,
            int? width = null,
            int? height = null)
        {
            Url = url;
            UrlType = urlType;
            ThumbnailUrl = thumbnailUrl;
            if (durationSeconds != null)
            {
                Duration = TimeSpan.FromSeconds((double) durationSeconds);
            }
            Width = width;
            Height = height;
        }
        
        public void Deconstruct(
            out string url,
            out UrlType urlType,
            out string thumbnailUrl,
            out TimeSpan? duration,
            out int? width,
            out int? height)
        {
            url = Url;
            urlType = UrlType;
            thumbnailUrl = ThumbnailUrl;
            duration = Duration;
            width = Width;
            height = Height;
        }
    }
}