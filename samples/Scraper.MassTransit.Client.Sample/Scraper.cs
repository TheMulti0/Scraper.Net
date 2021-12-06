using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Scraper.Net;

namespace Scraper.MassTransit.Client.Sample
{
    internal class Scraper : BackgroundService
    {
        private readonly IScraperService _scraper;
        private readonly ILogger<Scraper> _logger;

        public Scraper(IScraperService scraper, ILogger<Scraper> logger)
        {
            _scraper = scraper;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            const string platform = "facebook";
            const string id = "NaftaliBennett";

            // Author author = await _scraper.GetAuthorAsync(id, platform, stoppingToken);
            //
            // _logger.LogInformation(author.DisplayName);

            IAsyncEnumerable<Post> posts = _scraper.GetPostsAsync(id, platform, ct: stoppingToken);

            await foreach (Post post in posts.WithCancellation(stoppingToken))
            {
                _logger.LogInformation(post.Url);
            }
        }
    }
}