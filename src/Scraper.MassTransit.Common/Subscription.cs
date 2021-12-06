using System;

namespace Scraper.MassTransit.Common
{
    public sealed record Subscription
    {
        public string Platform { get; init; }

        public string Id { get; init; }

        public TimeSpan PollInterval { get; init; }
        
        public DateTime? NextPollTime { get; init; }

        public bool Equals(Subscription other) => other != null &&
                                                  Platform == other.Platform &&
                                                  Id == other.Id;
        public override int GetHashCode() => HashCode.Combine(Platform, Id);
    }
}