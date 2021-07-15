using System.Threading.Tasks;
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

        public async Task<ITweet[]> GetTweetsAsync(string userId)
        {
            var parameters = new GetUserTimelineParameters(userId)
            {
                PageSize = 10,
                TweetMode = TweetMode.Extended
            };
            
            return await TwitterClient.Timelines.GetUserTimelineAsync(parameters);
        }
    }
}