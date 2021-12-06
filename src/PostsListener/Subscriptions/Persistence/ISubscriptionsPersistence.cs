using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PostsListener
{
    public interface ISubscriptionsPersistence
    {
        IAsyncEnumerable<SubscriptionEntity> GetAsync(CancellationToken ct = default);
        
        Task<SubscriptionEntity> GetAsync(string id, string platform, CancellationToken ct = default);
        
        Task AddOrUpdateAsync(SubscriptionEntity subscription, CancellationToken ct = default);

        Task RemoveAsync(SubscriptionEntity subscription, CancellationToken ct = default);
    }
}