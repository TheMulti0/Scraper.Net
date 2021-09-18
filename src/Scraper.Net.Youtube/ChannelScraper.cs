using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using Microsoft.Extensions.Logging;

namespace Scraper.Net.Youtube
{
    internal class ChannelScraper
    {
        private readonly YouTubeService _service;
        private readonly ILogger<ChannelScraper> _logger;

        public ChannelScraper(
            YouTubeService service,
            ILogger<ChannelScraper> logger)
        {
            _service = service;
            _logger = logger;
        }

        public Task<Channel> GetChannelFromUsername(string username, CancellationToken ct)
        {
            return GetChannel(request => request.ForUsername = username, ct);
        }
        
        public Task<Channel> GetChannelFromId(string id, CancellationToken ct)
        {
            return GetChannel(request => request.Id = id, ct);
        }

        private async Task<Channel> GetChannel(Action<ChannelsResource.ListRequest> criteria, CancellationToken ct)
        {
            ChannelsResource.ListRequest request = _service.Channels.List(
                new[]
                {
                    "snippet",
                    "contentDetails"
                });
            criteria(request);

            ChannelListResponse response = await request.ExecuteAsync(ct);

            if (response.Items != null)
            {
                return response.Items.FirstOrDefault();
            }

            _logger.LogWarning("Channel list resulted in {}/{} results", response.PageInfo.ResultsPerPage, response.PageInfo.TotalResults);
            throw new NullReferenceException();
        }
    }
}