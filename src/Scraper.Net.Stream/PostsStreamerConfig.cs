using System;

namespace Scraper.Net.Stream
{
    public class PostsStreamerConfig
    {
        public int MaxDegreeOfParallelism { get; set; }

        public TimeSpan? PollingTimeout { get; set; }
    }
}