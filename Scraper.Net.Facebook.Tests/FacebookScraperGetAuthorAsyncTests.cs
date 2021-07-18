using System;
using System.Collections.Generic;
using System.Linq;
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
            var author = await _scraper.GetAuthorAsync(User);

            Assert.IsNotNull(author);
        }
        
        [TestMethod]
        public async Task TestError()
        {
            await Assert.ThrowsExceptionAsync<InvalidOperationException>(async () => await _scraper.GetAuthorAsync("myownrandomuser1234123123123123123123123123"));
        }
        
        [DataTestMethod]
        [DataRow(1)]
        [DataRow(500)]
        public async Task TestCancellation(int delayMs)
        {
            var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(delayMs));

            await Assert.ThrowsExceptionAsync<OperationCanceledException>(async () =>
            {
                // ReSharper disable once MethodSupportsCancellation
                await _scraper.GetAuthorAsync(User, cts.Token);
            });
        }
    }
}