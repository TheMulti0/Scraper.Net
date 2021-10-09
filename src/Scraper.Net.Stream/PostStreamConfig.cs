using System;

namespace Scraper.Net.Stream
{
    public class PostStreamConfig
    {
        public int MaxDegreeOfParallelism { get; set; }

        public TimeSpan? PollingTimeout { get; set; }
    }
}