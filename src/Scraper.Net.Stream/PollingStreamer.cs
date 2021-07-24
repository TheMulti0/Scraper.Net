using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading;

namespace Scraper.Net.Stream
{
    public static class PollingStreamer
    {
        public static IObservable<T> Stream<T>(
            Func<CancellationToken, IAsyncEnumerable<T>> asyncFunction,
            Func<T, bool> filter,
            TimeSpan interval)
        {
            IObservable<T> observable = Poll(
                asyncFunction,
                interval,
                Scheduler.Default);
            
            return observable
                .Retry() // Continue polling even if one batch threw an exception
                .Where(filter);
        } 

        private static IObservable<TResult> Poll<TResult>(
            Func<CancellationToken, IAsyncEnumerable<TResult>> asyncFunction,
            TimeSpan interval,
            IScheduler scheduler)
        {
            return Observable.Create<TResult>(observer =>
            {
                return scheduler.ScheduleAsync(async (s, ct) => 
                {
                    while(!ct.IsCancellationRequested)
                    {
                        try
                        {
                            await asyncFunction(ct).ForEachAsync(observer.OnNext, ct);
                        }
                        catch(Exception ex)
                        {
                            observer.OnError(ex);
                        }

                        await s.Sleep(interval, ct);
                    }
                });        
            });    
        }
    }
}