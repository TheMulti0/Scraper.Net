using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Scraper.Net;
using YoutubeDLSharp;
using YoutubeDLSharp.Metadata;

namespace Scraper.Net.YoutubeDl
{
    public class YoutubeDlPostProcessor : IPostProcessor
    {
        private readonly YoutubeDL _youtubeDl = new();

        public async IAsyncEnumerable<Post> ProcessAsync(Post post)
        {
            VideoItem videoItem = await ExtractVideoItem(post.Url);

            IEnumerable<IMediaItem> postMediaItems = post.MediaItems ?? Enumerable.Empty<IMediaItem>();
            
            IEnumerable<IMediaItem> mediaItems = postMediaItems
                .Where(item => item is not VideoItem)
                .Append(videoItem); // Append extracted video to media items
            
            yield return post with { MediaItems = mediaItems };
        }

        private async Task<VideoItem> ExtractVideoItem(string postUrl)
        {
            VideoData data = await GetVideoData(postUrl);

            FormatData highestFormat = GetHighestQualityFormat(data);
            ThumbnailData highestThumbnail = GetHighestQualityThumbnail(data);

            return new VideoItem(
                highestFormat.Url ?? data.Url,
                highestThumbnail.Url,
                data.Duration,
                highestFormat.Width ?? highestThumbnail.Width,
                highestFormat.Height ?? highestThumbnail.Height);
        }

        private static FormatData GetHighestQualityFormat(VideoData data)
        {
            const string none = "none";

            return data.Formats.LastOrDefault(
                format => format.AudioCodec != none && format.VideoCodec != none);
        }

        private static ThumbnailData GetHighestQualityThumbnail(VideoData data) => data.Thumbnails.LastOrDefault();

        private async Task<VideoData> GetVideoData(string url)
        {
            RunResult<VideoData> result = await _youtubeDl.RunVideoDataFetch(url);

            if (result.Success)
            {
                return result.Data;
            }
            
            string message = string.Join('\n', result.ErrorOutput);

            throw new Exception(message);
        }
    }
}