using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
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
        private readonly BehaviorSubject<DateTime?> _dueTime;

        public IObservable<Post> Posts { get; }

        public IObservable<DateTime?> DueTime => _dueTime;

        public PostStream(
            TimeSpan interval,
            TimeSpan? pollingTimeout,
            DateTime? nextPollTime,
            IScheduler scheduler,
            Func<CancellationToken, IAsyncEnumerable<Post>> pollAsync)
        {
            _dueTime = new BehaviorSubject<DateTime?>(nextPollTime);

            _intervalSubject = new IntervalSubject<Post>(
                pollingTimeout,
                scheduler ?? Scheduler.Default,
                _dueTime,
                async (observer, token) => await UpdateObserverAsync(observer, token).ToListAsync(token));
            
            Posts = _intervalSubject
                .RefCount();

            _interval = interval;
            _pollAsync = pollAsync;
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

            _dueTime.OnNext(DateTime.Now + _interval);
        }
    }
}