using System;

namespace Scraper.Net
{
    public record AudioItem : IMediaItem
    {
        public string Url { get; init; }
        public string ThumbnailUrl { get; init; }
        public TimeSpan? Duration { get; init; }
        public string Title { get; init; }
        public string Artist { get; init; }

        public AudioItem()
        {
        }

        public AudioItem(
            string url,
            string thumbnailUrl,
            TimeSpan? duration,
            string title,
            string artist)
        {
            Url = url;
            ThumbnailUrl = thumbnailUrl;
            Duration = duration;
            Title = title;
            Artist = artist;
        }

        public void Deconstruct(
            out string url,
            out string thumbnailUrl,
            out TimeSpan? duration,
            out string title,
            out string artist)
        {
            url = Url;
            thumbnailUrl = ThumbnailUrl;
            duration = Duration;
            title = Title;
            artist = Artist;
        }
    }
}