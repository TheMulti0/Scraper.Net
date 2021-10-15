using System;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;

namespace Scraper.Net.Stream
{
    internal class IntervalSubject<T> : IConnectableObservable<T>, ISubject<T>
    {
        private readonly Subject<T> _subject = new();
             
        private readonly TimeSpan? _updateTimeout;
        private readonly IScheduler _scheduler;
        private readonly IObservable<DateTime?> _dueTime;
        private readonly Func<IObserver<T>, CancellationToken, Task> _updateAsync;

        public IntervalSubject(
            TimeSpan? updateTimeout,
            IScheduler scheduler,
            IObservable<DateTime?> dueTime,
            Func<IObserver<T>, CancellationToken, Task> updateAsync)
        {
            _updateTimeout = updateTimeout;
            _scheduler = scheduler;
            _dueTime = dueTime;
            _updateAsync = updateAsync;
        }

        public IDisposable Connect()
        {
            async Task Loop(IScheduler s, CancellationToken ct)
            {
                while (!ct.IsCancellationRequested)
                {
                    IDisposable lastOperation = null;
                    
                    await foreach (DateTime? dueTime in _dueTime.ToAsyncEnumerable().WithCancellation(ct))
                    {
                        lastOperation?.Dispose();

                        lastOperation = dueTime == null 
                            ? _scheduler.ScheduleAsync(OnDueTime) 
                            : _scheduler.ScheduleAsync((DateTimeOffset) dueTime, OnDueTime);
                    }
                }
            }

            return _scheduler.ScheduleAsync(Loop);
        }

        private async Task OnDueTime(IScheduler scheduler, CancellationToken ct)
        {
            await _updateAsync(_subject, ct.WithTimeout(_updateTimeout));
        }

        public IDisposable Subscribe(IObserver<T> observer) => _subject.Subscribe(observer);
        
        public void OnCompleted() => _subject.OnCompleted();

        public void OnError(Exception error) => _subject.OnError(error);

        public void OnNext(T value) => _subject.OnNext(value);
    }
}