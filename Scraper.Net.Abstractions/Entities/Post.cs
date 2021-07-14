using System;
using System.Collections.Generic;

namespace Scraper.Net.Abstractions
{
    public record Post
    {
        public string Url { get; init; }

        public string Content { get; init; }

        public PostType Type { get; init; }

        public bool IsLivestream { get; init; }

        public User Author { get; init; }

        public DateTime? CreationDate { get; init; }
        
        public IEnumerable<IMediaItem> MediaItems { get; init; }
    }
}
