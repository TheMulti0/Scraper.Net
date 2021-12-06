using System;
using System.Linq;
using System.Threading.Tasks;
using MassTransit;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Scraper.Net;
using Scraper.Net.Stream;
using Scraper.MassTransit.Common;

namespace PostsListener.Tests
{
    public class DirectNewPostSubscriptionTests
    {
        private int _pollCounter = 0;
        private readonly InMemoryTestHarness _harness;
        private readonly IConsumerTestHarness<NewPostConsumer> _consumerHarness;
        private IRequestClient<AddOrUpdateNewPostSubscription> _addOrUpdate;
        private IRequestClient<RemoveNewPostSubscription> _remove;
        private IRequestClient<GetNewPostSubscriptions> _get;

        public DirectNewPostSubscriptionTests()
        {
            var provider = new ServiceCollection()
                .AddSingleton<IScraperService, MockScraperService>()
                .AddSingleton<ISubscriptionsManager, SubscriptionsManager>()
                .AddSingleton<ISubscriptionsPersistence, InMemorySubscriptionsPersistence>()
                .AddStream(async (post, platform, ct) => _pollCounter++ < 1)
                .AddSingleton<ILastPostsPersistence, InMemoryLastPostsPersistence>()
                .AddSingleton<IPostUrlsPersistence, InMemoryPostUrlsPersistence>()
                .AddLogging()
                .AddMassTransitInMemoryTestHarness(cfg =>
                {
                    cfg.AddConsumer<AddOrUpdateNewPostSubscriptionConsumer>();
                    cfg.AddConsumer<RemoveNewPostSubscriptionConsumer>();
                    cfg.AddConsumer<GetNewPostSubscriptionsConsumer>();
                    cfg.AddConsumer<NewPostConsumer>();
                })
                .BuildServiceProvider();

            _harness = provider.GetRequiredService<InMemoryTestHarness>();
            _consumerHarness = _harness.Consumer(() => provider.GetRequiredService<NewPostConsumer>());
            
            _harness.Consumer(() => provider.GetRequiredService<AddOrUpdateNewPostSubscriptionConsumer>());
            _harness.Consumer(() => provider.GetRequiredService<RemoveNewPostSubscriptionConsumer>());
            _harness.Consumer(() => provider.GetRequiredService<GetNewPostSubscriptionsConsumer>());
        }

        [OneTimeSetUp]
        public async Task SetUp()
        {
            await _harness.Start();
            
            _addOrUpdate = _harness.CreateRequestClient<AddOrUpdateNewPostSubscription>();
            _remove = _harness.CreateRequestClient<RemoveNewPostSubscription>();
            _get = _harness.CreateRequestClient<GetNewPostSubscriptions>();
        }

        [OneTimeTearDown]
        public async Task TearDown()
        {
            await _harness.Stop();
        }

        [Test]
        public async Task Test()
        {
            const string id = "";
            const string platform = "";
            
            var add = new AddOrUpdateNewPostSubscription
            {
                Id = id,
                Platform = platform,
                PollInterval = TimeSpan.FromDays(1)
            };

            Assert.IsNotNull(await _addOrUpdate.GetResponse<OperationSucceeded>(add));
            
            Assert.True(await _harness.Published.Any<NewPost>());
            Assert.True(await _consumerHarness.Consumed.Any<NewPost>());
            
            var remove = new RemoveNewPostSubscription
            {
                Id = id,
                Platform = platform
            };
            
            Assert.IsNotNull(await _remove.GetResponse<OperationSucceeded>(remove));

            var response = await _get.GetResponse<Subscriptions>(new GetNewPostSubscriptions());
            Assert.False(response.Message.Items.Any(subscription => subscription.Id == id && subscription.Platform == platform));
        }
    }
}