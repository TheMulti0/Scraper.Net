using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Logging;
using Scraper.Net;
using Scraper.Net.Stream;
using Scraper.MassTransit.Common;

namespace PostsListener
{
    public class StreamManager
    {
        private readonly PostStreamFactory _factory;
        private readonly IBus _bus;
        private readonly Dictionary<string, double> _intervalMultipliers;
        private readonly ConcurrentDictionary<Subscription, PostSubscription> _subscriptions;
        private readonly ILogger<StreamManager> _logger;

        public StreamManager(
            StreamConfig config,
            PostStreamFactory factory,
            IBus bus,
            ILogger<StreamManager> logger)
        {
            _factory = factory;
            _bus = bus;
            _intervalMultipliers = config.PlatformMultipliers;
            _subscriptions = new ConcurrentDictionary<Subscription, PostSubscription>();
            _logger = logger;
        }

        public IDictionary<Subscription, PostSubscription> Get()
        {
            return _subscriptions;
        }

        public PostSubscription AddOrUpdate(Subscription subscription, DateTime earliestPostDate)
        {
            if (_subscriptions.ContainsKey(subscription))
            {
                if (_subscriptions.FirstOrDefault(pair => pair.Key == subscription)
                    .Key.PollInterval != subscription.PollInterval)
                {
                    _subscriptions.Remove(subscription, out PostSubscription s);
                    s?.Dispose();
                }
            }

            return _subscriptions.GetOrAdd(
                subscription,
                s => StreamSubscription(s, earliestPostDate));    
        }

        private PostSubscription StreamSubscription(Subscription subscription, DateTime earliestPostDate)
        {
            var stream = CreatePostStream(subscription);

            var disposable = stream.Posts
                .Where(post => post.CreationDate > earliestPostDate)
                .SubscribeAsync(post => PublishPost(subscription.Platform, post));

            return new PostSubscription(stream, disposable);
        }

        private IPostStream CreatePostStream(Subscription subscription)
        {
            string id = subscription.Id;
            string platform = subscription.Platform;
            double intervalMultiplier = GetPlatformIntervalMultiplier(platform);
            TimeSpan interval = subscription.PollInterval * intervalMultiplier;
            DateTime? nextPollTime = subscription.NextPollTime;

            _logger.LogInformation("Streaming [{}] {} with interval of {} (nextPollTime is {})", platform, id, interval, nextPollTime);

            return _factory
                .Stream(id, platform, interval, nextPollTime);
        }

        private async Task PublishPost(string platform, Post post)
        {
            _logger.LogInformation("Sending {}", post.Url);

            await _bus.Publish(
                new NewPost
                {
                    Post = post,
                    Platform = platform
                });
        }

        private double GetPlatformIntervalMultiplier(string platform)
        {
            return _intervalMultipliers.ContainsKey(platform)
                ? _intervalMultipliers[platform]
                : 1;
        }

        public void Remove(Subscription subscription)
        {
            if (!_subscriptions.ContainsKey(subscription))
            {
                throw new KeyNotFoundException();
            }

            if (!_subscriptions.TryRemove(subscription, out PostSubscription postSubscription))
            {
                throw new InvalidOperationException("Failed to remove subscription");
            }

            postSubscription?.Dispose();
        }
    }
}