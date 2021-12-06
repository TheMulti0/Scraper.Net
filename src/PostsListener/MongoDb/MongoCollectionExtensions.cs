using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using MongoDB.Driver;

namespace PostsListener
{
    internal static class MongoCollectionExtensions
    {
        public static async IAsyncEnumerable<T> AsAsyncEnumerable<T>(
            this IMongoCollection<T> collection,
            FilterDefinition<T> filterDefinition,
            [EnumeratorCancellation] CancellationToken ct = default)
        {
            IAsyncCursor<T> asyncCursor = await collection.FindAsync(
                filterDefinition,
                cancellationToken: ct);

            while (await asyncCursor.MoveNextAsync(ct))
            {
                foreach (T item in asyncCursor.Current)
                {
                    yield return item;
                }
            }
        }
    }
}