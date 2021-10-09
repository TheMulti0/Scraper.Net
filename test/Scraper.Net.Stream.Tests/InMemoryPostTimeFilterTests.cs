using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Scraper.Net.Stream.Tests
{
    [TestClass]
    public class InMemoryPostTimeFilterTests
    {
        [TestMethod]
        public void TestFilter()
        {
            int year = 2000;

            for (int i = 0; i < 3; i++)
            {
                AssertFilterOfPostWithTime(false, null);
                AssertFilterOfPostWithTime(true, new DateTime(year, 1, 1));
                AssertFilterOfPostWithTime(false, new DateTime(year - 1, 1, 1));
                AssertFilterOfPostWithTime(true, new DateTime(year + 1, 1, 1));

                year += 10;
            }
        }

        private static async Task AssertFilterOfPostWithTime(bool expected, DateTime? creationDate)
        {
            bool filter = await InMemoryPostTimeFilter.Filter(
                new Post
                {
                    CreationDate = creationDate
                },
                "");

            Assert.AreEqual(expected, filter);
        }
    }
}