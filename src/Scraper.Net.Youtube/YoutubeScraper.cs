using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using MoreLinq;

namespace Scraper.Net.Youtube
{
    public class YoutubeScraper : IPlatformScraper
    {
        private readonly AsyncLazy<ChannelScraper> _channelScraper;
        private readonly AsyncLazy<VideosScraper> _videosScraper;

        public YoutubeScraper(YoutubeConfig config)
        {
            var youtubeService =
                new AsyncLazy<YouTubeService>(
                    () => new YouTubeService(
                        new BaseClientService.Initializer
                        {
                            ApiKey = config.ApiKey,
                            ApplicationName = config.AppName
                        }));

            _channelScraper = new AsyncLazy<ChannelScraper>(async () => new ChannelScraper(await youtubeService));
            _videosScraper = new AsyncLazy<VideosScraper>(async () => new VideosScraper(await youtubeService));
        }

        public async Task<Author> GetAuthorAsync(
            string id,
            CancellationToken ct = default)
        {
            Channel channel = await (await _channelScraper).GetChannelFromId(id, ct);
            ChannelSnippet snippet = channel.Snippet;

            return new Author
            {
                Id = channel.Id,
                DisplayName = snippet.Title,
                Description = snippet.Description,
                ProfilePictureUrl = snippet.Thumbnails.High.Url
            };
        }

        public IAsyncEnumerable<Post> GetPostsAsync(
            string id,
            CancellationToken ct = default)
        {
            Post ToPost(Video video)
            {
                VideoSnippet snippet = video.Snippet;

                var url = $"https://www.youtube.com/watch?v={video.Id}";

                VideoItem videoItem = GetVideoItem(video, url);

                return new Post
                {
                    AuthorId = id,
                    Content = $"{snippet.Title}\n \n{snippet.Description}",
                    Url = url,
                    CreationDate = snippet.PublishedAt,
                    IsLivestream = snippet.LiveBroadcastContent == "live",
                    MediaItems = new[]
                    {
                        videoItem
                    }
                };
            }

            return GetFullVideos(id, ct)
                .Select(ToPost);
        }

        private async IAsyncEnumerable<Video> GetFullVideos(
            string id,
            [EnumeratorCancellation] CancellationToken ct)
        {
            ChannelScraper channelScraper = await _channelScraper;
            Channel channel = await channelScraper.GetChannelFromId(id, ct);

            VideosScraper videosScraper = await _videosScraper;
            var videoIds = await GetVideoIds(videosScraper, channel.Id, ct);

            // Get videos by id works with up to 50 video ids at a time
            foreach (IEnumerable<string> videoIdsBatch in videoIds.Batch(50))
            {
                IEnumerable<Video> fullVideosBatch = await videosScraper.GetVideosById(
                    videoIdsBatch.ToArray(),
                    ct);
                
                foreach (Video video in fullVideosBatch)
                {
                    yield return video;
                }
            }
        }

        private static async Task<IEnumerable<string>> GetVideoIds(
            VideosScraper videosScraper,
            string channelId,
            CancellationToken ct)
        {
            IEnumerable<SearchResult> searchResults = await videosScraper.SearchVideosByChannelId(channelId, ct);

            return searchResults
                .Select(result => result.Id.VideoId);
        }

        private static VideoItem GetVideoItem(Video video, string url)
        {
            Thumbnail thumbnail = video.Snippet.Thumbnails.Maxres ??
                                  video.Snippet.Thumbnails.High ??
                                  video.Snippet.Thumbnails.Medium ??
                                  video.Snippet.Thumbnails.Standard ??
                                  video.Snippet.Thumbnails.Default__;

            var duration = XmlConvert.ToTimeSpan(video.ContentDetails.Duration);

            return new VideoItem(
                url,
                UrlType.WebpageUrl,
                thumbnail?.Url,
                duration,
                (int?)thumbnail?.Width,
                (int?)thumbnail?.Height);
        }
    }
}