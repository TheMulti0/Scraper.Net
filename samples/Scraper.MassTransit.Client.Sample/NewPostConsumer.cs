using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Logging;
using Scraper.MassTransit.Common;

namespace Scraper.MassTransit.Client.Sample
{
    internal class NewPostConsumer : IConsumer<NewPost>
    {
        private readonly ILogger<NewPostConsumer> _logger;

        public NewPostConsumer(ILogger<NewPostConsumer> logger)
        {
            _logger = logger;
        }

        public Task Consume(ConsumeContext<NewPost> context)
        {
            _logger.LogInformation(context.Message.Post.Url);

            return Task.CompletedTask;
        }
    }
}