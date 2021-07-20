using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Scraper.Net.Facebook.Tests
{
    [TestClass]
    public class FacebookScraperGetAuthorAsyncTests
    {
        private const string User = "ayelet.benshaul.shaked";
        private static FacebookScraper _scraper;

        [ClassInitialize]
        public static void Initialize(TestContext context)
        {
            _scraper = new FacebookScraper(
                new FacebookConfig());
        }

        [TestMethod]
        public async Task TestGet()
        {
            try
            {
                var author = await _scraper.GetAuthorAsync(User);

                Assert.IsNotNull(author);   
            }
            catch (LoginRequiredException)
            {
                Assert.Inconclusive("Login required");
            }
        }
        
        [TestMethod]
        public async Task TestError()
        {
            await Assert.ThrowsExceptionAsync<IdNotFoundException>(async () => await _scraper.GetAuthorAsync("myownrandomuser1234123123123123123123123123"));
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
                await Assert.ThrowsExceptionAsync<OperationCanceledException>(
                    async () =>
                    {
                        // ReSharper disable once MethodSupportsCancellation
                        await _scraper.GetAuthorAsync(User, cts.Token);
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