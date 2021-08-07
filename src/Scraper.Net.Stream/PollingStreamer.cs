using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Scraper.Net.Stream
{
    internal static class PollingStreamer
    {
        public static IObservable<T> Stream<T>(
            Func<CancellationToken, IAsyncEnumerable<T>> asyncFunction,
            TimeSpan interval)
        {
            IObservable<T> stream = Poll(
                asyncFunction,
                interval);

            return stream.Retry(); // Continue polling even if one batch threw an exception
        } 

        private static IObservable<TResult> Poll<TResult>(
            Func<CancellationToken, IAsyncEnumerable<TResult>> asyncFunction,
            TimeSpan interval)
        {
            async Task PollLoop(IObserver<TResult> observer, CancellationToken ct)
            {
                while (!ct.IsCancellationRequested)
                {
                    try
                    {
                        await asyncFunction(ct)
                            .ForEachAsync(observer.OnNext, ct);
                    }
                    catch (Exception ex)
                    {
                        observer.OnError(ex);
                    }

                    await Task.Delay(interval, ct);
                }
            }

            return Observable.Create<TResult>(PollLoop);
        }
    }
}