using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
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
            IObservable<Unit> trigger,
            TimeSpan? pollingTimeout = null,
            IScheduler scheduler = null)
        {
            scheduler ??= Scheduler.Default;
            
            IObservable<T> stream = Poll(
                asyncFunction,
                trigger,
                pollingTimeout,
                scheduler);
            

            // Continue polling even if one batch threw an exception, except IdNotFoundException which breaks the stream
            return stream
                .RetryWhen(
                    exceptions => exceptions.Select(e =>
                    {
                        if (e is IdNotFoundException)
                            throw e;
                        return e;
                    }));
        }

        private static IObservable<TResult> Poll<TResult>(
            Func<CancellationToken, IAsyncEnumerable<TResult>> asyncFunction,
            IObservable<Unit> trigger,
            TimeSpan? pollingTimeout,
            IScheduler scheduler)
        {
            IDisposable SchedulePollLoop(IObserver<TResult> observer)
            {
                async Task PollLoop(CancellationToken cancellationToken)
                {
                    CancellationToken ct = GetCancellationToken(cancellationToken, pollingTimeout);

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
                    }
                }

                return trigger.SubscribeAsync(PollLoop, scheduler);
            }

            return Observable.Create<TResult>(SchedulePollLoop);
        }

        private static CancellationToken GetCancellationToken(CancellationToken original, TimeSpan? timeout)
        {
            if (timeout == null)
            {
                return original;
            }
            
            var cts = new CancellationTokenSource();
            original.Register(cts.Cancel);
            return cts.Token;
        }
    }
}