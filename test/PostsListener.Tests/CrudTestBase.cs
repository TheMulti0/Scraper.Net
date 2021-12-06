using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;

namespace PostsListener.Tests
{
    [TestFixture]
    public class CrudTestBase<T>
    {
        private readonly Func<T> _factory;
        private readonly Func<CancellationToken, IAsyncEnumerable<T>> _get;
        private readonly Func<T, CancellationToken, Task> _add;
        private readonly Func<T, CancellationToken, Task> _remove;

        public CrudTestBase(
            Func<T> factory,
            Func<CancellationToken, IAsyncEnumerable<T>> get,
            Func<T, CancellationToken, Task> add,
            Func<T, CancellationToken, Task> remove)
        {
            _factory = factory;
            _get = get;
            _add = add;
            _remove = remove;
        }
        
        [Test]
        public async Task TestAddSingleAsync()
        {
            await ClearAsync();
            
            Assert.IsFalse(await _get(default).AnyAsync());

            await _add(_factory(), default);

            Assert.AreEqual(1, await _get(default).CountAsync());
        }
        
        [Test]
        public async Task TestAddRemoveSingleAsync()
        {
            await ClearAsync();
            
            Assert.IsFalse(await _get(default).AnyAsync());

            await _add(_factory(), default);

            Assert.AreEqual(1, await _get(default).CountAsync());

            await _remove(_factory(), default);
            
            Assert.IsFalse(await _get(default).AnyAsync());
        }

        public async Task ClearAsync()
        {
            await foreach (T t in _get(default))
            {
                await _remove(t, default);
            }
        }
    }
}