using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Scraper.Net.Facebook.Tests
{
    internal class FacebookScraperTestHelper
    {
        private const string AssertExceptionPattern = @"Assert.ThrowsException failed.\sThrew\sexception\s(\w+Exception)";
        private static readonly Regex AssertExceptionRegex = new(AssertExceptionPattern);

        public static bool DidThrow<T>(AssertFailedException e)
        {
            return AssertExceptionRegex.Match(e.Message).Groups[1].Value == nameof(T);
        }
    }
}