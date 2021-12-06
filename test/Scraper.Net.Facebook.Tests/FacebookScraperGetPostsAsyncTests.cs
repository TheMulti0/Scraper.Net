using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Scraper.Net.Facebook.Tests
{
    [TestClass]
    public class FacebookScraperGetPostsAsyncTests
    {
        private const string User = "yomtovkalfon";
        private static FacebookScraper _scraper;

        [ClassInitialize]
        public static void Initialize(TestContext context)
        {
            _scraper = new FacebookScraper(
                new FacebookConfig
                {
                    MaxPageCount = 3
                },
                NullLoggerFactory.Instance);
        }

        [TestMethod]
        public async Task TestGet()
        {
            try
            {
                List<Post> posts = await _scraper.GetPostsAsync(User).ToListAsync();

                Assert.IsNotNull(posts);
                Assert.IsTrue(posts.Any());
                CollectionAssert.AllItemsAreNotNull(posts);    
            }
            catch (LoginRequiredException)
            {
                Assert.Inconclusive("Login required");
            }
        }
        
        [TestMethod]
        public async Task TestError()
        {
            await Assert.ThrowsExceptionAsync<IdNotFoundException>(async () => await _scraper.GetPostsAsync("anonexistinguser123135435332423").ToListAsync());
        }
        
        [DataTestMethod]
        [DataRow(1)]
        [DataRow(500)]
        [DataRow(5000)]
        public async Task TestCancellation(int delayMs)
        {
            var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(delayMs));

            try
            {
                await Assert.ThrowsExceptionAsync<OperationCanceledException>(async () =>
                {
                    // ReSharper disable once MethodSupportsCancellation
                    await _scraper.GetPostsAsync(User, cts.Token).ToListAsync();
                });
            }
            catch (AssertFailedException e)
            {
                if (FacebookScraperTestHelper.DidThrow<LoginRequiredException>(e))
                {
                    Assert.Inconclusive("Login required");
                }
            }
        }
    }
}