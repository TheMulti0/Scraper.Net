namespace Scraper.Net
{
    /// <summary>
    /// Represents the base unit of a post author 
    /// </summary>
    public record Author
    {
        public string Id { get; init; }

        public string DisplayName { get; init; }
        
        public string ProfilePictureUrl { get; init; }

        public string Description { get; init; }
    }
}