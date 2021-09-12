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
            (_, _) => Task.FromResult(true),
            new PostsStreamerConfig(),
            NullLogger<PostsStreamer>.Instance);
        
        [DataTestMethod]
        [DataRow(50, 1)]
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

            await Task.Delay(interval * expectedPostCount * 5);

            Assert.AreEqual(expectedPostCount, actualPostCount);
        }

        [TestMethod]
        public async Task TestIdNotFoundException()
        {
            TimeSpan interval = TimeSpan.FromSeconds(1);

            var obs = _streamer
                .Stream("noid", "", interval)
                .Take(1);
            
            await Assert.ThrowsExceptionAsync<IdNotFoundException>(async () => await obs);
        }
        
        [TestMethod]
        public async Task TestOneTimeException()
        {
            TimeSpan interval = TimeSpan.FromSeconds(1);

            var obs = _streamer
                .Stream("onetime", "", interval)
                .Take(1);
            
            await obs; // Should not throw an exception 
        }

        [DataTestMethod]
        [DataRow(1)]
        [DataRow(4)]
        [DataRow(-1)]
        public async Task TestMaxDegreeOfParallelism(int max)
        {
            PostsStreamer streamer = new(
                new SinglePostScraperService(),
                (_, _) => Task.FromResult(true),
                new PostsStreamerConfig
                {
                    MaxDegreeOfParallelism = max 
                },
                NullLogger<PostsStreamer>.Instance);
            
            const int expected = 1;
            int actualPostCount = 0;

            streamer
                .Stream("", "", TimeSpan.FromDays(1))
                .Take(expected)
                .Subscribe(
                    _ =>
                    {
                        lock (this)
                        {
                            actualPostCount++;
                        }
                    });

            await Task.Delay(100);
            
            Assert.AreEqual(expected, actualPostCount);
        }
    }
}