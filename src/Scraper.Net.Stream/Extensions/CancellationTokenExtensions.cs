using System;
using System.Threading;

namespace Scraper.Net.Stream
{
    public static class CancellationTokenExtensions
    {
        public static CancellationToken WithTimeout(this CancellationToken original, TimeSpan? timeout)
        {
            if (timeout == null)
            {
                return original;
            }
            
            var cts = new CancellationTokenSource((TimeSpan) timeout);
            
            original.Register(cts.Cancel);
            
            return cts.Token;
        }
    }
}