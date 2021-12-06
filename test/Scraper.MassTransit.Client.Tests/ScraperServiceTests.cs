using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NUnit.Framework;
using Scraper.MassTransit.Common;
using Scraper.Net;

namespace Scraper.MassTransit.Client.Tests
{
    public class ScraperServiceTests
    {
        private readonly IHostedService _massTransitService;
        private readonly IScraperService _client;

        public ScraperServiceTests()
        {
            var services = new ServiceCollection()
                .AddLogging()
                .AddScraperMassTransitClient()
                .AddMassTransit(
                    x =>
                    {
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
            _client = provider.GetRequiredService<IScraperService>();
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
        public async Task TestGetPosts()
        {
            const string id = "NaftaliBennett";
            const string platform = "facebook";

            List<Post> posts = await _client
                .GetPostsAsync(id, platform)
                .ToListAsync();
            CollectionAssert.IsNotEmpty(posts);
            CollectionAssert.AllItemsAreNotNull(posts);
        }
        
        [Test]
        public async Task TestGetAuthor()
        {
            const string id = "NaftaliBennett";
            const string platform = "facebook";

            var author = await _client.GetAuthorAsync(id, platform);
            
            Assert.IsNotNull(author);
        }
    }
}