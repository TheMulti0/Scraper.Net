using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Scraper.Net.Twitter
{
    public static class RegexExtensions
    {
        public static async Task<string> ReplaceAsync(
            this Regex regex,
            string input,
            Func<Match, Task<string>> replacementFn,
            CancellationToken ct = default)
        {
            var sb = new StringBuilder();
            var lastIndex = 0;

            foreach (Match match in regex.Matches(input))
            {
                ct.ThrowIfCancellationRequested();

                sb.Append(input, lastIndex, match.Index - lastIndex)
                    .Append(await replacementFn(match).ConfigureAwait(false));

                lastIndex = match.Index + match.Length;
            }

            sb.Append(input, lastIndex, input.Length - lastIndex);
            return sb.ToString();
        }
    }
}