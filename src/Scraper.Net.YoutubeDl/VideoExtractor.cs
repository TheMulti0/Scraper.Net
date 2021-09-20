using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using YoutubeDLSharp;
using YoutubeDLSharp.Metadata;
using YoutubeDLSharp.Options;

namespace Scraper.Net.YoutubeDl
{
    public class VideoExtractor
    {
        private readonly YoutubeDL _youtubeDl;
        private readonly OptionSet _overrideOptions;

        public VideoExtractor(
            YoutubeDL youtubeDl,
            OptionSet overrideOptions)
        {
            _youtubeDl = youtubeDl;
            _overrideOptions = overrideOptions;
        }

        public async Task<YoutubeDlVideoItem> ExtractAsync(
            string url,
            CancellationToken ct = default)
        {
            VideoData data = await ExceptionHandler.Do(() => GetVideoData(url, ct));

            FormatData highestFormat = GetHighestQualityFormat(data);
            ThumbnailData highestThumbnail = GetHighestQualityThumbnail(data);
            
            return new YoutubeDlVideoItem(
                highestFormat?.Url ?? data.Url,
                UrlType.DirectUrl,
                highestThumbnail.Url,
                highestFormat?.FileSize ?? highestFormat?.ApproximateFileSize,
                data.Duration,
                highestFormat?.Width ?? highestThumbnail.Width,
                highestFormat?.Height ?? highestThumbnail.Height);
        }

        private static FormatData GetHighestQualityFormat(VideoData data)
        {
            const string noCodecName = "none";

            return data.Formats?
                .OrderBy(format => format.Bitrate)
                .FirstOrDefault(format => format.AudioCodec != noCodecName &&
                                          format.VideoCodec != noCodecName);
        }

        private static ThumbnailData GetHighestQualityThumbnail(VideoData data) 
            => data.Thumbnails
                .OrderBy(thumbnailData => thumbnailData.Width)
                .FirstOrDefault();

        private async Task<VideoData> GetVideoData(string url, CancellationToken ct)
        {
            RunResult<VideoData> result = await _youtubeDl.RunVideoDataFetch(url, ct, overrideOptions: _overrideOptions);

            if (result.Success && result.Data?.Url != null)
            {
                return result.Data;
            }
            
            string message = string.Join('\n', result.ErrorOutput);
            throw new YoutubeDlException(message);
        }
    }
}