using System.Threading.Tasks;
using MassTransit;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Scraper.MassTransit.Common;
using Scraper.Net;
using Assert = NUnit.Framework.Assert;

namespace Scraper.MassTransit.Tests
{
    public class GetPostsConsumerTests
    {
        private readonly InMemoryTestHarness _harness;
        private readonly IConsumerTestHarness<GetPostsConsumer> _consumerHarness;
        private IRequestClient<GetPosts> _requestClient;

        public GetPostsConsumerTests()
        {
            var provider = new ServiceCollection()
                .AddSingleton<IScraperService, MockScraperService>()
                .AddLogging()
                .AddMassTransitInMemoryTestHarness(cfg => cfg.AddConsumer<GetPostsConsumer>())
                .BuildServiceProvider();

            _harness = provider.GetRequiredService<InMemoryTestHarness>();
            _consumerHarness = _harness.Consumer(() => provider.GetRequiredService<GetPostsConsumer>());
        }

        [OneTimeSetUp]
        public async Task SetUp()
        {
            await _harness.Start();
            
            _requestClient = _harness.CreateRequestClient<GetPosts>();
        }

        [OneTimeTearDown]
        public async Task TearDown()
        {
            await _harness.Stop();
        }

        [Test]
        public async Task Test()
        {
            var request = new GetPosts
            {
                Id = "",
                Platform = "" 
            };
            
            var response = await _requestClient.GetResponse<OperationStarted>(request);
            
            Assert.IsNotNull(response.Message);
            
            Assert.True(await _harness.Published.Any<Post>());
        }
    }
}