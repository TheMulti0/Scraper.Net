namespace Scraper.Net.Facebook.Scraper
{
    public record Stats
    {
        public int Comments { get; init; }

        public int Shares { get; init; }

        public int Likes { get; init; }
    }
}