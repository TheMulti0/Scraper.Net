namespace Scraper.Net
{
    public record PostAuthor
    {
        public string Id { get; init; }

        public string DisplayName { get; init; }

        public string Url { get; init; }
    }
}