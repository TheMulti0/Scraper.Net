using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Google.Apis.Util;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;

namespace Scraper.Net.Youtube
{
    internal class VideosScraper
    {
        private readonly YouTubeService _service;

        public VideosScraper(YouTubeService service)
        {
            _service = service;
        }

        public async Task<IEnumerable<SearchResult>> SearchVideosByChannelId(string channelId, CancellationToken ct)
        {
            SearchResource.ListRequest request = _service.Search.List(
                new[]
                {
                    "id",
                    "snippet"
                });
            request.ChannelId = channelId;
            request.Type = "video";
            request.Order = SearchResource.ListRequest.OrderEnum.Date;

            SearchListResponse response = await request.ExecuteAsync(ct);
            return response.Items;
        }

        public async Task<IEnumerable<Video>> GetVideosById(Repeatable<string> ids, CancellationToken ct)
        {
            VideosResource.ListRequest request = _service.Videos.List(
                new[]
                {
                    "snippet",
                    "contentDetails"
                });
            request.Id = ids;

            VideoListResponse response = await request.ExecuteAsync(ct);
            return response.Items;
        } 
    }
}