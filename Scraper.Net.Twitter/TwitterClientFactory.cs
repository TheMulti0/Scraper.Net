using System.Threading.Tasks;
using Tweetinvi;

namespace Scraper.Net.Twitter
{
    internal class TwitterClientFactory
    {
        public static async Task<ITwitterClient> CreateAsync(TwitterConfig config)
        {
            var twitterClient = new TwitterClient(
                config.ConsumerKey,
                config.ConsumerSecret);

            await twitterClient.Auth.InitializeClientBearerTokenAsync();

            return twitterClient;
        }
    }
}