using System;
using System.Collections.Generic;
using System.Linq;

namespace Scraper.Net
{
    /// <summary>
    /// Represents the base unit of a post 
    /// </summary>
    public record Post
    {
        public string Url { get; init; }

        public string Content { get; init; }

        public PostType Type { get; init; }

        public bool IsLivestream { get; init; }

        public string AuthorId { get; init; }

        public DateTime? CreationDate { get; init; }

        public IEnumerable<IMediaItem> MediaItems { get; init; } = Enumerable.Empty<IMediaItem>();
    }
}
