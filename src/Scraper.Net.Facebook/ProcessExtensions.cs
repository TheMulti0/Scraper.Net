using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Scraper.Net.Facebook
{
    internal static class ProcessExtensions
    {
        public static IAsyncEnumerable<string> StandardOutput(this Process process)
        {
            return GetOutput(process, process.StandardOutput);
        }
        
        public static IAsyncEnumerable<string> StandardError(this Process process)
        {
            return GetOutput(process, process.StandardError);
        }

        private static async IAsyncEnumerable<string> GetOutput(
            Process process,
            StreamReader stream)
        {
            while (stream.EndOfStream == false)
            {
                yield return await stream.ReadLineAsync();
            }

            await process.WaitForExitAsync();
        }
    }
}