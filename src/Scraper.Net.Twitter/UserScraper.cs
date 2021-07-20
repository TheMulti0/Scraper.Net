using System.Threading.Tasks;
using Tweetinvi;
using Tweetinvi.Models;
using Tweetinvi.Parameters;

namespace Scraper.Net.Twitter
{
    internal class UserScraper
    {
        private readonly ITwitterClient _client;
        
        public UserScraper(ITwitterClient client)
        {
            _client = client;
        }

        public Task<IUser> GetUserAsync(string userId)
        {
            var parameters = new GetUserParameters(userId)
            {
                User = new UserIdentifier(userId)
            };

            return _client.Users.GetUserAsync(parameters);
        }
    }
}