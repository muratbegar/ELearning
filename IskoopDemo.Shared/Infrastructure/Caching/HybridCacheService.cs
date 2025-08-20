using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace IskoopDemo.Shared.Infrastructure.Caching
{
    public class HybridCacheService : ICacheService
    {

        private readonly IMemoryCache _memoryCache;
        private readonly IDistributedCache _distributedCache;
        private readonly ILogger<HybridCacheService> _logger;
        private readonly JsonSerializerOptions _jsonOptions;
        private readonly TimeSpan _memoryCacheExpiration = TimeSpan.FromMinutes(5); // Default expiration time

        public HybridCacheService(IMemoryCache memoryCache,IDistributedCache distributedCache,ILogger<HybridCacheService> logger)
        {
            _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
            _distributedCache = distributedCache ?? throw new ArgumentNullException(nameof(distributedCache));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            };
        }

        public async Task<T> GetAsync<T>(string key, CancellationToken cancellationToken = default)
        {
            try
            {
                // Try memory cache first (L1)
                if (_memoryCache.TryGetValue(key, out var memoryValue))
                {
                    _logger?.LogDebug("L1 Cache hit for key: {Key}", key);
                    return (T)memoryValue;
                }

                // Try distributed cache (L2)
                var distributedValue = await _distributedCache.GetStringAsync(key, cancellationToken);
                if (!string.IsNullOrEmpty(distributedValue))
                {
                    _logger?.LogDebug("L2 Cache hit for key: {Key}", key);
                    var deserializedValue = JsonSerializer.Deserialize<T>(distributedValue, _jsonOptions);

                    // Store in memory cache for faster future access
                    _memoryCache.Set(key, deserializedValue, _memoryCacheExpiration);

                    return deserializedValue;
                }

                _logger?.LogDebug("Cache miss for key: {Key}", key);
                return default(T);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error getting value from hybrid cache for key: {Key}", key);
                return default(T);
            }
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default)
        {
            try
            {
                // Set in memory cache (L1)
                var memoryExpiration = expiration > _memoryCacheExpiration ? _memoryCacheExpiration : expiration;
                _memoryCache.Set(key, value, memoryExpiration.Value);

                // Set in distributed cache (L2)
                var serializedValue = JsonSerializer.Serialize(value, _jsonOptions);
                var options = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = expiration
                };

                await _distributedCache.SetStringAsync(key, serializedValue, options, cancellationToken);
                _logger?.LogDebug("Hybrid cache set for key: {Key}, expiration: {Expiration}", key, expiration);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error setting value in hybrid cache for key: {Key}", key);
            }
        }

        public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
        {
            try
            {
                // Remove from both caches
                _memoryCache.Remove(key);
                await _distributedCache.RemoveAsync(key, cancellationToken);
                _logger?.LogDebug("Hybrid cache removed for key: {Key}", key);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error removing value from hybrid cache for key: {Key}", key);
            }
        }

        public async Task RemoveByPatternAsync(string pattern, CancellationToken cancellationToken = default)
        {
            // This would require Redis-specific implementation for distributed cache
            _logger?.LogWarning("Pattern-based cache removal requires Redis-specific implementation: {Pattern}", pattern);
        }

        public async Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default)
        {
            try
            {
                // Check memory cache first
                if (_memoryCache.TryGetValue(key, out _))
                    return true;

                // Check distributed cache
                var value = await _distributedCache.GetStringAsync(key, cancellationToken);
                return !string.IsNullOrEmpty(value);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error checking existence in hybrid cache for key: {Key}", key);
                return false;
            }
        }
    }
}
