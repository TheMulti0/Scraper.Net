using System.Collections.Generic;
using System.Linq;
using Tweetinvi;
using Tweetinvi.Models;
using Tweetinvi.Parameters;

namespace Scraper.Net.Twitter
{
    internal class TweetScraper
    {
        internal ITwitterClient TwitterClient { get; }
        
        public TweetScraper(
            TwitterScraperConfig config)
        {
            TwitterClient = new TwitterClient(
                config.ConsumerKey,
                config.ConsumerSecret);

            TwitterClient.Auth.InitializeClientBearerTokenAsync().Wait();
        }

        public IAsyncEnumerable<ITweet> GetTweetsAsync(string userId)
        {
            const int pageSize = 10;
            const int pageCount = 5;

            var parameters = new GetUserTimelineParameters(userId)
            {
                PageSize = pageSize,
                TweetMode = TweetMode.Extended
            };

            return TwitterClient.Timelines.GetUserTimelineIterator(parameters)
                .ToAsyncEnumerable()
                .Take(pageSize * pageCount);
        }
    }
}