using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;

namespace Scraper.Net.Youtube
{
    internal class ChannelScraper
    {
        private readonly YouTubeService _service;

        public ChannelScraper(YouTubeService service)
        {
            _service = service;
        }

        public Task<Channel> GetChannelFromUsername(string username, CancellationToken ct)
        {
            return GetChannel(request => request.ForUsername = username, ct);
        }
        
        public Task<Channel> GetChannelFromId(string id, CancellationToken ct)
        {
            return GetChannel(request => request.Id = id, ct);
        }

        private async Task<Channel> GetChannel(Action<ChannelsResource.ListRequest> set, CancellationToken ct)
        {
            ChannelsResource.ListRequest request = _service.Channels.List(
                new[]
                {
                    "snippet",
                    "contentDetails"
                });
            set(request);

            ChannelListResponse response = await request.ExecuteAsync(ct);
            
            return response.Items.FirstOrDefault();
        }
    }
}