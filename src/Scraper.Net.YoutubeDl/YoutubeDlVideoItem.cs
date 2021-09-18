using System.Text.Json.Serialization;

namespace Scraper.Net.YoutubeDl
{
    public record YoutubeDlVideoItem : VideoItem
    {
        public long? FileSize { get; }

        [JsonConstructor]
        public YoutubeDlVideoItem(
            string url,
            UrlType urlType,
            string thumbnailUrl,
            long? fileSize,
            double? duration,
            int? width,
            int? height) : base(url, urlType, thumbnailUrl, duration, width, height)
        {
            FileSize = fileSize;
        }
    }
}