using System;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Scraper.Net.Stream.Tests
{
    [TestClass]
    public class PostsStreamerTests
    {
        private readonly PostStreamFactory _streamer = new(
            new SinglePostScraperService(),
            (_, _, _) => Task.FromResult(true),
            new PostStreamConfig(),
            NullLogger<IPostStream>.Instance);

        private readonly TimeSpan _oneInterval = TimeSpan.FromDays(1);
        private readonly TimeSpan _recurringInterval = TimeSpan.FromMilliseconds(100);

        [DataTestMethod]
        [DataRow(50, 1)]
        [DataRow(50, 5)]
        public void TestStreamingWithSinglePostBatch(int intervalMs, int expectedPostCount)
        {
            var interval = TimeSpan.FromMilliseconds(intervalMs);

            var actualPostCount = _streamer
                .Stream("", "", interval)
                .Posts
                .Take(expectedPostCount)
                .Timeout(interval * expectedPostCount)
                .ToEnumerable()
                .Count();

            Assert.AreEqual(expectedPostCount, actualPostCount);
        }

        [TestMethod]
        public async Task TestIdNotFoundException()
        {
            var obs = _streamer
                .Stream("noid", "", _oneInterval)
                .Posts
                .Take(1);
            
            await Assert.ThrowsExceptionAsync<IdNotFoundException>(async () => await obs);
        }
        
        [TestMethod]
        public async Task TestOneTimeException()
        {
            var obs = _streamer
                .Stream("onetime", "", _recurringInterval)
                .Posts
                .Take(1);
            
            await obs; // Should not throw an exception 
        }
        
        [TestMethod]
        public async Task TestOneTimeExceptionRecovery()
        {
            var obs = _streamer
                .Stream("onetime", "", _recurringInterval)
                .Posts
                .Take(2);
            
            await obs; // Should not throw an exception 
        }

        [DataTestMethod]
        [DataRow(1)]
        [DataRow(4)]
        [DataRow(-1)]
        public void TestMaxDegreeOfParallelism(int max)
        {
            PostStreamFactory streamer = new(
                new SinglePostScraperService(),
                (_, _, _) => Task.FromResult(true),
                new PostStreamConfig
                {
                    MaxDegreeOfParallelism = max 
                },
                NullLogger<IPostStream>.Instance);
            
            const int expected = 1;

            var post = streamer
                .Stream("", "", _oneInterval)
                .Posts
                .Take(expected)
                .Timeout(_recurringInterval)
                .FirstOrDefaultAsync();
            
            Assert.IsNotNull(post);
        }

        [TestMethod]
        public async Task TestOrderingByDate()
        {
            var multipleStreamer = new PostStreamFactory(
                new MultiplePostsScraperService(),
                (_, _, _) => Task.FromResult(true),
                new PostStreamConfig(),
                NullLogger<IPostStream>.Instance);

            IObservable<Post> observable = multipleStreamer
                .Stream("", "", _oneInterval)
                .Posts;

            Post first = await observable.FirstAsync();
            Post last = await observable.FirstAsync();
            Assert.IsTrue(first.CreationDate < last.CreationDate);
        }
        
        [TestMethod]
        public async Task TestSingleSubscription()
        {
            var count = 2;
            
            var stream = _streamer
                .Stream("", "", _recurringInterval)
                .Posts
                .Take(count);

            var subject1 = new Subject<Post>();
            var subject2 = new Subject<Post>();
            
            stream.Subscribe(subject1);
            stream.Subscribe(subject2);

            for (int i = 0; i < count; i++)
            {
                Assert.AreEqual((await subject1.FirstAsync()).CreationDate, (await subject2.FirstAsync()).CreationDate);
            }
        }
    }
}