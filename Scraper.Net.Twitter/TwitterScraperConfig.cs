namespace Scraper.Net.Twitter
{
    public record TwitterScraperConfig
    {
        public string ConsumerKey { get; init; }

        public string ConsumerSecret { get; init; }

        public int MaxPageSize { get; init; } = 200;

        public int MaxPages { get; init; } = 1;
    }
}