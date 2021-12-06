using System.Collections.Generic;

namespace Scraper.MassTransit.Common
{
    public record Subscriptions
    {
        public IEnumerable<Subscription> Items { get; init; }
    }
}