using System;

namespace Scraper.Net.Facebook.Scraper
{
    public record Video
    {
        public string Id { get; init; }

        public string Url { get; init; }

        public TimeSpan? Duration { get; init; }

        public int? Width { get; init; }

        public int? Height { get; init; }

        public string Quality { get; init; }

        public string ThumbnailUrl { get; init; }

        public double? SizeMb { get; init; }

        public int? Watches { get; init; }
    }
}