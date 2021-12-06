using System.Threading;
using System.Threading.Tasks;
using MassTransit;
using Scraper.MassTransit.Common;
using Scraper.Net;

namespace Scraper.MassTransit
{
    public class GetAuthorConsumer : IConsumer<GetAuthor>
    {
        private readonly IScraperService _scraperService;

        public GetAuthorConsumer(IScraperService scraperService)
        {
            _scraperService = scraperService;
        }

        public async Task Consume(ConsumeContext<GetAuthor> context)
        {
            GetAuthor request = context.Message;
            CancellationToken ct = context.CancellationToken;
            
            var author = await _scraperService.GetAuthorAsync(
                request.Id,
                request.Platform,
                ct);

            await context.RespondAsync(author);
        }
    }
}