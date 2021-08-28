namespace Scraper.Net
{
    public record PhotoItem : IMediaItem
    {
        public string Url { get; init; }

        public PhotoItem()
        {
        }

        public PhotoItem(string url)
        {
            Url = url;
        }

        public void Deconstruct(out string url)
        {
            url = Url;
        }
    }
}