using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Logging;
using Scraper.MassTransit.Common;

namespace PostsListener
{
    public class RemoveNewPostSubscriptionConsumer : IConsumer<RemoveNewPostSubscription>
    {
        private readonly ISubscriptionsManager _subscriptionsManager;
        private readonly ILogger<RemoveNewPostSubscriptionConsumer> _logger;

        public RemoveNewPostSubscriptionConsumer(
            ISubscriptionsManager subscriptionsManager,
            ILogger<RemoveNewPostSubscriptionConsumer> logger)
        {
            _subscriptionsManager = subscriptionsManager;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<RemoveNewPostSubscription> context)
        {
            RemoveNewPostSubscription request = context.Message;
            string id = request.Id;
            string platform = request.Platform;
            
            var subscription = new Subscription
            {
                Platform = platform,
                Id = id
            };
            
            await _subscriptionsManager.RemoveAsync(subscription, context.CancellationToken);
            
            _logger.LogInformation("Unsubscribed to [{}] {}", platform, id);

            await context.RespondAsync(OperationSucceeded.Instance);
        }
    }
}