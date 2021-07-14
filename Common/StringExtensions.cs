using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Common
{
    public static class StringExtensions
    {
        public static string Replace(
            this string input,
            IEnumerable<string> patterns,
            string replacement)
        {
            string newestText = input;
            
            foreach (string pattern in patterns)
            {
                newestText = Regex.Replace(newestText, pattern, replacement);
            }

            return newestText;
        }
    }
}