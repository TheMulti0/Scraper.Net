using System;
using System.Collections.Generic;
using System.Linq;
using Tweetinvi;
using Tweetinvi.Models;
using Tweetinvi.Parameters;

namespace Scraper.Net.Twitter
{
    internal class TweetScraper
    {
        private readonly ITwitterClient _client;
        private readonly TwitterConfig _config;
        
        public TweetScraper(ITwitterClient client, TwitterConfig config)
        {
            _client = client;
            _config = config;
        }

        public IAsyncEnumerable<ITweet> GetTweetsAsync(string userId)
        {
            var parameters = new GetUserTimelineParameters(userId)
            {
                PageSize = _config.MaxPageSize, 
                TweetMode = TweetMode.Extended
            };

            return _client.Timelines.GetUserTimelineIterator(parameters)
                .ToAsyncEnumerable()
                .Take(_config.MaxPageCount * _config.MaxPageSize);
        }
    }
}