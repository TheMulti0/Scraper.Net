namespace Scraper.Net.Twitter
{
    public record TwitterConfig
    {
        public string ConsumerKey { get; init; }

        public string ConsumerSecret { get; init; }

        public int MaxPageSize { get; init; } = 200;

        public int MaxPageCount { get; init; } = 1;
    }
}