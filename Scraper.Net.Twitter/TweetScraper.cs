using System.Collections.Generic;
using System.Linq;
using Tweetinvi;
using Tweetinvi.Models;
using Tweetinvi.Parameters;

namespace Scraper.Net.Twitter
{
    internal class TweetScraper
    {
        private readonly TwitterScraperConfig _config;
        internal ITwitterClient TwitterClient { get; }
        
        public TweetScraper(
            TwitterScraperConfig config)
        {
            _config = config;
            
            TwitterClient = new TwitterClient(
                _config.ConsumerKey,
                _config.ConsumerSecret);

            TwitterClient.Auth.InitializeClientBearerTokenAsync().Wait();
        }

        public IAsyncEnumerable<ITweet> GetTweetsAsync(string userId)
        {
            var parameters = new GetUserTimelineParameters(userId)
            {
                PageSize = _config.MaxPageSize, 
                TweetMode = TweetMode.Extended
            };

            return TwitterClient.Timelines.GetUserTimelineIterator(parameters)
                .ToAsyncEnumerable()
                .Take(_config.MaxPages * _config.MaxPageSize);
        }
    }
}