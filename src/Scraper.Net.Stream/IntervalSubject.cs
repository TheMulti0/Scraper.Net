using System;
using System.Reactive.Concurrency;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;

namespace Scraper.Net.Stream
{
    internal class IntervalSubject<T> : IConnectableObservable<T>, ISubject<T>
    {
        private readonly Subject<T> _subject = new();
        private readonly Func<IObserver<T>, CancellationToken, Task> _update;
        private readonly TimeSpan _interval;
        private readonly TimeSpan? _updateTimeout;
        private readonly IScheduler _scheduler;

        public IntervalSubject(
            TimeSpan interval,
            TimeSpan? updateTimeout,
            IScheduler scheduler,
            Func<IObserver<T>, CancellationToken, Task> update)
        {
            _update = update;
            _interval = interval;
            _updateTimeout = updateTimeout;
            _scheduler = scheduler;
        }

        public IDisposable Connect()
        {
            async Task Loop(IScheduler s, CancellationToken ct)
            {
                while (!ct.IsCancellationRequested)
                {
                    await _update(_subject, ct.WithTimeout(_updateTimeout));

                    await s.Sleep(_interval, ct);
                }
            }

            return _scheduler.ScheduleAsync(Loop);
        }

        public IDisposable Subscribe(IObserver<T> observer) => _subject.Subscribe(observer);
        
        public void OnCompleted() => _subject.OnCompleted();

        public void OnError(Exception error) => _subject.OnError(error);

        public void OnNext(T value) => _subject.OnNext(value);
    }
}