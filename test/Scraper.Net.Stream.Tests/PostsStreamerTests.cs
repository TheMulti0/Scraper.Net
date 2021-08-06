using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Scraper.Net.Stream.Tests
{
    [TestClass]
    public class PostsStreamerTests
    {
        private readonly PostsStreamer _streamer = new(
            new SinglePostScraperService(),
            (_, _) => true,
            NullLogger<PostsStreamer>.Instance);
        
        [DataTestMethod]
        [DataRow(10, 1)]
        [DataRow(10, 10)]
        public async Task TestStreamingWithSinglePostBatch(int intervalMs, int expectedPostCount)
        {
            var interval = TimeSpan.FromMilliseconds(intervalMs);

            int actualPostCount = 0;

            _streamer
                .Stream("", "", interval)
                .Take(expectedPostCount)
                .Subscribe(
                    _ =>
                    {
                        lock (this)
                        {
                            actualPostCount++;
                        }
                    });

            await Task.Delay(interval * expectedPostCount * 1.5);

            Assert.AreEqual(expectedPostCount, actualPostCount);
        }
    }
}