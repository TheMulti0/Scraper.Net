using System;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PostsListener
{
    internal static class PostSubscriptionExtensions
    {
        public static IDisposable SaveNewDueTimes(
            this PostSubscription postSubscription,
            SubscriptionEntity original,
            Func<SubscriptionEntity, CancellationToken, Task> saveAsync)
        {
            return postSubscription.DueTime
                .Where(dueTime => dueTime != null && dueTime > original.NextPollTime)
                .Select(dueTime => original with { NextPollTime = dueTime})
                .SubscribeAsync(saveAsync);
        }
    }
}