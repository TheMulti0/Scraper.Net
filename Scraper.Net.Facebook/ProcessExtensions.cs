using System;
using System.Diagnostics;
using System.Reactive.Linq;

namespace Scraper.Net.Facebook
{
    internal static class ProcessExtensions
    {
        public static IObservable<string> StandardOutput(this Process process)
        {
            return Observable.Create<string>(
                async (observer, ct) =>
                {
                    while (!process.StandardOutput.EndOfStream)
                    {
                        string line = await process.StandardOutput.ReadLineAsync();
                        observer.OnNext(line);
                    }

                    await process.WaitForExitAsync(ct);
                    observer.OnCompleted();
                });
        }
    }
}