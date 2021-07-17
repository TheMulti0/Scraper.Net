using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
// ReSharper disable MethodSupportsCancellation

namespace Scraper.Net.Tests
{
    [TestClass]
    public class ScraperCancellationTests
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

        [DataTestMethod]
        [DataRow(500)]
        [DataRow(5000)]
        public async Task TestPostProcessorImmediateCancellation(int delayMs)
        {
            await TestPostProcessorCancellation(delayMs, 0.1);
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
            async Task Test()
            {
                TimeSpan delay = TimeSpan.FromMilliseconds(delayMs);

                var scraper = new Scraper(
                    new Dictionary<string, IPlatformScraper>
                    {
                        {"mock", new MockDelayScraper(delay)}
                    },
                    new List<IPostProcessor>());

                var cts = new CancellationTokenSource(delay * coefficient);

                try
                {
                    await scraper.GetPostsAsync("mockuser", "mock", cts.Token)
                        .ToListAsync();
                }
                catch (TaskCanceledException e)
                {
                    var scraperMethod = $"{nameof(MockDelayScraper)}.{nameof(MockDelayScraper.GetPostsAsync)}";
                    Assert.IsTrue(e.StackTrace.Contains(scraperMethod));

                    throw;
                }
            }

            await Assert.ThrowsExceptionAsync<TaskCanceledException>(Test);
        }

        private static async Task TestPostProcessorCancellation(int delayMs, double coefficient)
        {
            async Task Scrape()
            {
                TimeSpan delay = TimeSpan.FromMilliseconds(delayMs);

                var scraper = new Scraper(
                    new Dictionary<string, IPlatformScraper>
                    {
                        {"mock", new MockScraper(true)}
                    },
                    new List<IPostProcessor>
                    {
                        new MockDelayPostProcessor(delay)
                    });

                var cts = new CancellationTokenSource(delay * coefficient);

                try
                {
                    await scraper.GetPostsAsync("mockuser", "mock", cts.Token)
                        .ToListAsync();
                }
                catch (TaskCanceledException e)
                {
                    var postProcessorMethod = $"{nameof(MockDelayPostProcessor)}.{nameof(MockDelayPostProcessor.ProcessAsync)}";
                    Assert.IsTrue(e.StackTrace.Contains(postProcessorMethod));

                    throw;
                }
            }

            await Assert.ThrowsExceptionAsync<TaskCanceledException>(Scrape);
        }
    }
}