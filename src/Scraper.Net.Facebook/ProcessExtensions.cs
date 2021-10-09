using System;
using System.Diagnostics;
using System.IO;
using System.Reactive.Concurrency;
using System.Reactive.Linq;

namespace Scraper.Net.Facebook
{
    internal static class ProcessExtensions
    {
        public static IObservable<string> StandardOutput(this Process process, IScheduler scheduler = null) 
            => GetOutput(process, process.StandardOutput, scheduler);

        public static IObservable<string> StandardError(this Process process, IScheduler scheduler = null) 
            => GetOutput(process, process.StandardError, scheduler);

        private static IObservable<string> GetOutput(
            Process process,
            StreamReader stream,
            IScheduler scheduler)
        {
            scheduler ??= Scheduler.Default;

            IDisposable Observe(IObserver<string> observer)
            {
                return scheduler.ScheduleAsync(
                    async (s, ct) =>
                    {
                        while (stream.EndOfStream == false)
                        {
                            string line = await stream.ReadLineAsync();
                            observer.OnNext(line);
                        }

                    await process.WaitForExitAsync(ct);
                    observer.OnCompleted();
                });
            }

            return Observable.Create<string>(Observe);
        }
    }
}