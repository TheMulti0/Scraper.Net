using System;
using System.Linq;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Logging;
using Scraper.MassTransit.Common;

namespace PostsListener
{
    public class PollNewPostSubscriptionConsumer : IConsumer<PollNewPostSubscription>
    {
        private readonly StreamManager _streamManager;
        private readonly ILogger<PollNewPostSubscriptionConsumer> _logger;

        public PollNewPostSubscriptionConsumer(
            StreamManager streamManager,
            ILogger<PollNewPostSubscriptionConsumer> logger)
        {
            _streamManager = streamManager;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<PollNewPostSubscription> context)
        {
            PollNewPostSubscription request = context.Message;
            string id = request.Id;
            string platform = request.Platform;

            (Subscription _, PostSubscription subscription) = _streamManager.Get()
                .First(pair => pair.Key.Id == id && pair.Key.Platform == platform);
            
            _logger.LogInformation("Triggering poll for [{}] {}", platform, id);
            
            await subscription.UpdateAsync(context.CancellationToken).ToListAsync(context.CancellationToken);
            
            await context.RespondAsync(OperationSucceeded.Instance);
        }
    }
}