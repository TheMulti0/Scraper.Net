namespace Scraper.MassTransit.Common
{
    public record RemoveNewPostSubscription
    {
        public string Platform { get; init; }

        public string Id { get; init; }
    }
}