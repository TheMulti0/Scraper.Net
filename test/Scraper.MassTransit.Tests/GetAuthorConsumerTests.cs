using System.Threading.Tasks;
using MassTransit;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Scraper.MassTransit.Common;
using Scraper.Net;

namespace Scraper.MassTransit.Tests
{
    public class GetAuthorConsumerTests
    {
        private readonly InMemoryTestHarness _harness;
        private readonly IConsumerTestHarness<GetAuthorConsumer> _consumerHarness;
        private IRequestClient<GetAuthor> _requestClient;

        public GetAuthorConsumerTests()
        {
            var provider = new ServiceCollection()
                .AddSingleton<IScraperService, MockScraperService>()
                .AddLogging()
                .AddMassTransitInMemoryTestHarness(cfg => cfg.AddConsumer<GetAuthorConsumer>())
                .BuildServiceProvider();

            _harness = provider.GetRequiredService<InMemoryTestHarness>();
            _consumerHarness = _harness.Consumer(() => provider.GetRequiredService<GetAuthorConsumer>());
        }

        [OneTimeSetUp]
        public async Task SetUp()
        {
            await _harness.Start();
            
            _requestClient = _harness.CreateRequestClient<GetAuthor>();
        }

        [OneTimeTearDown]
        public async Task TearDown()
        {
            await _harness.Stop();
        }

        [Test]
        public async Task Test()
        {
            var request = new GetAuthor
            {
                Id = "",
                Platform = "" 
            };
            
            var response = await _requestClient.GetResponse<Author>(request);
            
            Assert.IsNotNull(response.Message);
        }
    }
}