using System.Threading;
using System.Threading.Tasks;

namespace PostsListener
{
    public interface IPostUrlsPersistence
    {
        Task<bool> ExistsAsync(string url, CancellationToken ct = default);

        Task AddAsync(string url, CancellationToken ct = default);

        Task RemoveAsync(string url, CancellationToken ct = default);
    }
}