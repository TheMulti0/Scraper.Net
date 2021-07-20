namespace Scraper.Net.Facebook
{
    public record Image
    {
        public string Id { get; init; }
        
        public string Url { get; init; }

        public string LowQualityUrl { get; init; }
        
        public string Description { get; init; }
    }
}