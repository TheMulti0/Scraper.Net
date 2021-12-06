using System;

namespace Scraper.MassTransit.Common
{
    public record AddOrUpdateNewPostSubscription
    {
        public string Platform { get; init; }

        public string Id { get; init; }

        public TimeSpan PollInterval { get; init; }

        public DateTime EarliestPostDate { get; init; }
    }
}