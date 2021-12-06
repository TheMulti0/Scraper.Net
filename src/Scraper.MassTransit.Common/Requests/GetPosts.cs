namespace Scraper.MassTransit.Common
{
    public record GetPosts
    {
        public string Id { get; init; }

        public string Platform { get; init; }
    }
}