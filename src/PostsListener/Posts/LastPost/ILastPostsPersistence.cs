using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PostsListener
{
    public interface ILastPostsPersistence
    {
        IAsyncEnumerable<LastPost> GetAsync(CancellationToken ct = default);

        Task<LastPost> GetAsync(string platform, string authorId, CancellationToken ct = default);

        Task AddOrUpdateAsync(string platform, string authorId, DateTime lastPostTime, CancellationToken ct = default);

        Task RemoveAsync(LastPost lastPost, CancellationToken ct = default);
    }
}