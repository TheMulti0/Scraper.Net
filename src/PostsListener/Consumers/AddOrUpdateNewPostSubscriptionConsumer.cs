using System;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Logging;
using Scraper.MassTransit.Common;

namespace PostsListener
{
    public class AddOrUpdateNewPostSubscriptionConsumer : IConsumer<AddOrUpdateNewPostSubscription>
    {
        private readonly ISubscriptionsManager _subscriptionsManager;
        private readonly ILogger<AddOrUpdateNewPostSubscriptionConsumer> _logger;

        public AddOrUpdateNewPostSubscriptionConsumer(
            ISubscriptionsManager subscriptionsManager,
            ILogger<AddOrUpdateNewPostSubscriptionConsumer> logger)
        {
            _subscriptionsManager = subscriptionsManager;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<AddOrUpdateNewPostSubscription> context)
        {
            AddOrUpdateNewPostSubscription request = context.Message;
            string id = request.Id;
            string platform = request.Platform;
            TimeSpan pollInterval = request.PollInterval;
            
            if (pollInterval <= TimeSpan.Zero)
            {
                throw new ArgumentNullException(nameof(pollInterval));
            }
            
            var subscription = new Subscription
            {
                Platform = platform,
                Id = id,
                PollInterval = pollInterval
            };
            
            await _subscriptionsManager.AddOrUpdateAsync(subscription, request.EarliestPostDate, context.CancellationToken);
            
            _logger.LogInformation("Subscribed to [{}] {} with interval of {}", platform, id, pollInterval);

            await context.RespondAsync(OperationSucceeded.Instance);
        }
    }
}