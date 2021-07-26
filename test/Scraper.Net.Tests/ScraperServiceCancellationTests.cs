using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
// ReSharper disable MethodSupportsCancellation

namespace Scraper.Net.Tests
{
    [TestClass]
    public class ScraperServiceCancellationTests
    {
        [DataTestMethod]
        [DataRow(500)]
        [DataRow(5000)]
        public async Task TestScraperImmediateCancellation(int delayMs)
        {
            await TestPlatformScraperCancellation(delayMs, 0.1);
        }
        
        [DataTestMethod]
        [DataRow(500)]
        [DataRow(5000)]
        public async Task TestScraperDelayedCancellation(int delayMs)
        {
            await TestPlatformScraperCancellation(delayMs, 1.1);
        }

        [TestMethod]
        public async Task TestPostProcessorImmediateCancellation()
        {
            await TestPostProcessorCancellation(0, 0.1);
        }

        [DataTestMethod]
        [DataRow(500)]
        [DataRow(5000)]
        public async Task TestPostProcessorDelayedCancellation(int delayMs)
        {
            await TestPostProcessorCancellation(delayMs, 1.1);
        }

        private static async Task TestPlatformScraperCancellation(int delayMs, double coefficient)
        {
            TimeSpan delay = TimeSpan.FromMilliseconds(delayMs);

            var scraper = new ScraperService(
                new Dictionary<string, IPlatformScraper>
                {
                    {"mock", new MockDelayScraper(delay)}
                },
                new List<PostFilter>(),
                new List<IPostProcessor>(),
                NullLogger<ScraperService>.Instance);

            var cts = new CancellationTokenSource(delay * coefficient);


            async Task Test()
            {
                await scraper.GetPostsAsync("mockuser", "mock", cts.Token)
                    .ToListAsync();
            }

            var exception = await Assert.ThrowsExceptionAsync<TaskCanceledException>(Test);
            
            var scraperMethod = $"{nameof(MockDelayScraper)}.{nameof(MockDelayScraper.GetPostsAsync)}";
            Assert.IsTrue(exception.StackTrace.Contains(scraperMethod));
            
        }

        private static async Task TestPostProcessorCancellation(int delayMs, double coefficient)
        {
            async Task Scrape()
            {
                TimeSpan delay = TimeSpan.FromMilliseconds(delayMs);

                var scraper = new ScraperService(
                    new Dictionary<string, IPlatformScraper>
                    {
                        {"mock", new MockScraper(true)}
                    },
                    new List<PostFilter>(),
                    new List<IPostProcessor>
                    {
                        new MockDelayPostProcessor(delay)
                    },
                    NullLogger<ScraperService>.Instance);

                var cts = new CancellationTokenSource(delay * coefficient);

                try
                {
                    await scraper.GetPostsAsync("mockuser", "mock", cts.Token)
                        .ToListAsync();
                }
                catch (TaskCanceledException e)
                {
                    throw new OperationCanceledException(string.Empty, e);
                }
            }

            await Assert.ThrowsExceptionAsync<OperationCanceledException>(Scrape);
        }
    }
}