using System.Threading;
using System.Threading.Tasks;

namespace Scraper.Net.Facebook
{
    internal class ProxyManager
    {
        private readonly FacebookConfig _config;
        private readonly SemaphoreSlim _proxyIndexLock;
        private int _proxyIndex;

        public ProxyManager(FacebookConfig config)
        {
            _config = config;
            _proxyIndexLock = new SemaphoreSlim(1, 1);
        }
        public async Task<string> GetProxyAsync(CancellationToken ct)
        {
            if (_config.Proxies.Length == 0)
            {
                return null;
            }
            
            await _proxyIndexLock.WaitAsync(ct);

            try
            {
                if (_proxyIndex == _config.Proxies.Length - 1)
                {
                    _proxyIndex = 0;
                }
                else
                {
                    _proxyIndex++;
                }
                
                return _config.Proxies[_proxyIndex];
            }
            finally
            {
                _proxyIndexLock.Release();
            }
        }
        
    }
}