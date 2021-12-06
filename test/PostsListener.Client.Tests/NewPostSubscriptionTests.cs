using System;
using System.Threading;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NUnit.Framework;
using Scraper.MassTransit.Common;
using Scraper.Net;

namespace PostsListener.Client.Tests
{
    [TestFixture]
    public class NewPostSubscriptionTests
    {
        private readonly INewPostSubscriptionsClient _client;
        private readonly NewPostCounter _counter;
        private readonly IHostedService _massTransitService;

        public NewPostSubscriptionTests()
        {
            var services = new ServiceCollection()
                .AddLogging()
                .AddSingleton<NewPostCounter>()
                .AddMassTransit(
                    x =>
                    {
                        x.AddPostsListenerClient<NewPostConsumer>();

                        x.UsingRabbitMq(
                            (context, cfg) =>
                            {
                                RabbitMqConfig rabbitMqConfig = new();
                                cfg.Host(rabbitMqConfig.ConnectionString);

                                cfg.ConfigureInterfaceJsonSerialization(typeof(IMediaItem));

                                cfg.ConfigureEndpoints(context);
                            });
                    })
                .AddMassTransitHostedService();
            
            var provider = services.BuildServiceProvider();

            _massTransitService = provider.GetRequiredService<IHostedService>();
            _client = provider.GetRequiredService<INewPostSubscriptionsClient>();
            _counter = provider.GetRequiredService<NewPostCounter>();
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            await _massTransitService.StartAsync(CancellationToken.None);
        }

        [OneTimeTearDown]
        public async Task TearDown()
        {
            await _massTransitService.StopAsync(CancellationToken.None);
        }
        
        [Test]
        public async Task Test()
        {
            const string id = "NaftaliBennett";
            const string platform = "facebook";

            await _client.AddOrUpdateSubscription(id, platform, TimeSpan.FromDays(1), DateTime.MinValue);

            await Task.Delay(TimeSpan.FromSeconds(20));
            
            Assert.Greater(_counter.Get(), 0);

            await _client.RemoveSubscription(id, platform);
        }
    }
}