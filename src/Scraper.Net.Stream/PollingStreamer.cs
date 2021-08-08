using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Scraper.Net.Stream
{
    internal static class PollingStreamer
    {
        public static IObservable<T> Stream<T>(
            Func<CancellationToken, IAsyncEnumerable<T>> asyncFunction,
            TimeSpan interval,
            IScheduler scheduler)
        {
            scheduler ??= Scheduler.Default;
            
            IObservable<T> stream = Poll(
                asyncFunction,
                interval,
                scheduler);

            return stream.Retry(); // Continue polling even if one batch threw an exception
        } 

        private static IObservable<TResult> Poll<TResult>(
            Func<CancellationToken, IAsyncEnumerable<TResult>> asyncFunction,
            TimeSpan interval,
            IScheduler scheduler)
        {
            IDisposable SchedulePollLoop(IObserver<TResult> observer)
            {
                async Task PollLoop(IScheduler s, CancellationToken ct)
                {
                    while (!ct.IsCancellationRequested)
                    {
                        try
                        {
                            IAsyncEnumerable<TResult> asyncEnumerable = asyncFunction(ct);

                            await asyncEnumerable.ForEachAsync(observer.OnNext, ct);
                        }
                        catch (Exception ex)
                        {
                            observer.OnError(ex);
                        }

                        await s.Sleep(interval, ct);
                    }
                }

                return scheduler.ScheduleAsync(PollLoop);
            }

            return Observable.Create<TResult>(SchedulePollLoop);
        }
    }
}