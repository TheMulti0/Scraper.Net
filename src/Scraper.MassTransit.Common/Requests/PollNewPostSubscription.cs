namespace Scraper.MassTransit.Common
{
    public record PollNewPostSubscription
    {
        public string Platform { get; init; }

        public string Id { get; init; }
    }
}