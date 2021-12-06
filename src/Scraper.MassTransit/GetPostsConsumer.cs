using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MassTransit;
using Scraper.MassTransit.Common;
using Scraper.Net;

namespace Scraper.MassTransit
{
    public class GetPostsConsumer : IConsumer<GetPosts>
    {
        private readonly IScraperService _scraperService;

        public GetPostsConsumer(IScraperService scraperService)
        {
            _scraperService = scraperService;
        }

        public async Task Consume(ConsumeContext<GetPosts> context)
        {
            GetPosts request = context.Message;
            CancellationToken ct = context.CancellationToken;
            
            IAsyncEnumerable<Post> posts = _scraperService.GetPostsAsync(
                request.Id,
                request.Platform,
                ct);

            await foreach (Post post in posts.WithCancellation(ct))
            {
                await context.RespondAsync(post);
            }
            
            await context.RespondAsync(OperationSucceeded.Instance);
        }
    }
}