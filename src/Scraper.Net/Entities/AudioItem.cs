using System;

namespace Scraper.Net
{
    public record AudioItem(
        string Url,
        string ThumbnailUrl,
        TimeSpan? Duration,
        string Title,
        string Artist) : IMediaItem;
}