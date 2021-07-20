using System.Collections.Generic;
using System.Diagnostics;

namespace Scraper.Net.Facebook
{
    internal static class ProcessExtensions
    {
        public static async IAsyncEnumerable<string> StandardOutput(this Process process)
        {
            while (process?.StandardOutput.EndOfStream == false)
            {
                string line = await process.StandardOutput.ReadLineAsync();
                yield return line;
            }

            if (process != null)
            {
                await process.WaitForExitAsync();
            }
        }
    }
}