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
        
        private readonly TimeSpan _interval;
        private readonly TimeSpan? _updateTimeout;
        private readonly IScheduler _scheduler;
        private readonly Func<TimeSpan> _getRemainingSleepTime;
        private readonly Func<IObserver<T>, CancellationToken, Task> _updateAsync;

        public IntervalSubject(
            TimeSpan interval,
            TimeSpan? updateTimeout,
            IScheduler scheduler,
            Func<TimeSpan> getRemainingSleepTime,
            Func<IObserver<T>, CancellationToken, Task> updateAsync)
        {
            _interval = interval;
            _updateTimeout = updateTimeout;
            _scheduler = scheduler;
            _getRemainingSleepTime = getRemainingSleepTime;
            _updateAsync = updateAsync;
        }

        public IDisposable Connect()
        {
            async Task Loop(IScheduler s, CancellationToken ct)
            {
                while (!ct.IsCancellationRequested)
                {
                    await s.Sleep(_getRemainingSleepTime(), ct);

                    await _updateAsync(_subject, ct.WithTimeout(_updateTimeout));
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