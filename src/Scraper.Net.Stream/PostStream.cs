using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Scraper.Net.Stream
{
    internal class PostStream : IPostStream
    {
        private readonly IntervalSubject<Post> _intervalSubject;
        private readonly TimeSpan _interval;
        private readonly Func<CancellationToken, IAsyncEnumerable<Post>> _pollAsync;
        
        public DateTime? NextPollTime { get; private set; }

        public IObservable<Post> Posts { get; }

        public PostStream(
            TimeSpan interval,
            TimeSpan? pollingTimeout,
            DateTime? nextPollTime,
            IScheduler scheduler,
            Func<CancellationToken, IAsyncEnumerable<Post>> pollAsync,
            Func<Post, Task<bool>> filter)
        {
            NextPollTime = nextPollTime;

            _intervalSubject = new IntervalSubject<Post>(
                interval,
                pollingTimeout,
                scheduler ?? Scheduler.Default,
                GetRemainingSleepTime,
                async (observer, token) => await UpdateObserverAsync(observer, token).ToListAsync(token));
            
            Posts = _intervalSubject
                .RefCount()
                .WhereAwait(filter);

            _interval = interval;
            _pollAsync = pollAsync;
        }

        private TimeSpan GetRemainingSleepTime()
        {
            if (NextPollTime == null || NextPollTime <= DateTime.Now)
            {
                return TimeSpan.Zero;
            }

            return (DateTime) NextPollTime - DateTime.Now;
        }

        public IAsyncEnumerable<Post> UpdateAsync(CancellationToken ct) => UpdateObserverAsync(_intervalSubject, ct);

        private async IAsyncEnumerable<Post> UpdateObserverAsync(
            IObserver<Post> observer,
            [EnumeratorCancellation] CancellationToken ct)
        {
            void HandleException(Exception exception)
            {
                if (exception is IdNotFoundException)
                {
                    observer.OnError(exception);
                }
            }

            IAsyncEnumerable<Post> filtered = _pollAsync(ct).Catch<Post, Exception>(HandleException);

            await foreach (Post post in filtered.WithCancellation(ct))
            {
                observer.OnNext(post);

                yield return post;
            }

            NextPollTime = DateTime.Now + _interval;
        }
    }
}