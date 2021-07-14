using System;

namespace Scraper.Net.Abstractions
{
    public record AudioItem(
        string Url,
        string ThumbnailUrl,
        TimeSpan? Duration,
        string Title,
        string Artist) : IMediaItem;
}