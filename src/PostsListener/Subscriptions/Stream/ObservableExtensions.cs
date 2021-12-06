using System;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PostsListener
{
    public static class ObservableExtensions
    {
        public static IDisposable SubscribeAsync<T>(
            this IObservable<T> source,
            Func<T, Task> onNextAsync)
        {
            return source
                .SelectMany(value => Observable.FromAsync(() => onNextAsync(value)))
                .Subscribe();
        }

        public static IDisposable SubscribeAsync<T>(
            this IObservable<T> source,
            Func<T, CancellationToken, Task> onNextAsync)
        {
            return source
                .SelectMany(value => Observable.FromAsync(ct => onNextAsync(value, ct)))
                .Subscribe();
        }
    }
}