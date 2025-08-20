using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IskoopDemo.Shared.Infrastructure.Caching
{
    public class MemoryCacheService : ICacheService
    {
        private readonly IMemoryCache _memoryCache;
        private readonly ILogger<MemoryCacheService> _logger;

        public MemoryCacheService(IMemoryCache memoryCache,ILogger<MemoryCacheService> logger)
        {
            _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<T> GetAsync<T>(string key, CancellationToken cancellationToken = default)
        {
            try
            {
                if (_memoryCache.TryGetValue(key, out var value))
                {
                    _logger?.LogDebug("Cache hit for key: {Key}", key);
                    return (T)value;
                }
                _logger?.LogDebug("Cache miss for key: {Key}", key);
                return default(T);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error getting value from cache for key: {Key}", key);
                return default(T);
            }
        }

        public  async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default)
        {
            try
            {
                var options = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = expiration,
                    SlidingExpiration = TimeSpan.FromMinutes(5), // Reset expiration if accessed within 5 minutes
                    Priority = CacheItemPriority.Normal
                };

                _memoryCache.Set(key, value, options);
                _logger?.LogDebug("Cache set for key: {Key}, expiration: {Expiration}", key, expiration);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error setting value in cache for key: {Key}", key);
            }
        }

        public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
        {
            try
            {
                _memoryCache.Remove(key);
                _logger?.LogDebug("Cache removed for key: {Key}", key);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error removing value from cache for key: {Key}", key);
            }
        }

        public async Task RemoveByPatternAsync(string pattern, CancellationToken cancellationToken = default)
        {
            // MemoryCache doesn't support pattern-based removal natively
            // This would require a custom implementation or wrapper
            _logger?.LogWarning("Pattern-based cache removal not supported in MemoryCache: {Pattern}", pattern);
        }

        public async Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default)
        {
            return _memoryCache.TryGetValue(key, out _);
        }
    }
}
