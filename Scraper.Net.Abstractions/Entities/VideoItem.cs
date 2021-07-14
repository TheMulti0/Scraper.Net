using System;

namespace Scraper.Net.Abstractions
{
    public record VideoItem : IMediaItem
    {
        public string Url { get; }
        public string ThumbnailUrl { get; }
        public TimeSpan? Duration { get; }
        public int? Width { get; }
        public int? Height { get; }

        public VideoItem(
            string url,
            string thumbnailUrl,
            TimeSpan? duration = null,
            int? width = null,
            int? height = null)
        {
            Url = url;
            ThumbnailUrl = thumbnailUrl;
            Duration = duration;
            Width = width;
            Height = height;
        }
    }
}