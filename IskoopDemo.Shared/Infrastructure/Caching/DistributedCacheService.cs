using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace IskoopDemo.Shared.Infrastructure.Caching
{
    public class DistributedCacheService : ICacheService
    {
        private readonly IDistributedCache _distributedCache;
        private readonly ILogger<DistributedCacheService> _logger;
        private readonly JsonSerializerOptions _jsonOptions;


        public DistributedCacheService(IDistributedCache distributedCache,ILogger<DistributedCacheService> logger)
        {
            _distributedCache = distributedCache ?? throw new ArgumentNullException(nameof(distributedCache));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = false,
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
            };
        }

        public async Task<T> GetAsync<T>(string key, CancellationToken cancellationToken = default)
        {
            try
            {
                var cachedValue = await _distributedCache.GetStringAsync(key, cancellationToken);
                if (string.IsNullOrEmpty(cachedValue))
                {
                    _logger?.LogDebug("Cache miss for key: {Key}", key);
                    return default(T);
                }
                _logger?.LogDebug("Cache hit for key: {Key}", key);
                return JsonSerializer.Deserialize<T>(cachedValue, _jsonOptions);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error getting value from distributed cache for key: {Key}", key);
                return default(T);
            }
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default)
        {
            try
            {
                var serializedValue = JsonSerializer.Serialize(value, _jsonOptions);
                var options = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = expiration ?? TimeSpan.FromMinutes(60), // Default to 60 minutes if no expiration is provided
                    SlidingExpiration = TimeSpan.FromMinutes(5) // Reset expiration if accessed within 5 minutes
                };

                await _distributedCache.SetStringAsync(key, serializedValue, options, cancellationToken);
                _logger?.LogDebug("Cache set for key: {Key}, expiration: {Expiration}", key, expiration);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error setting value in distributed cache for key: {Key}", key);
               
            }
        }

        public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
        {
            try
            {
                await _distributedCache.RemoveAsync(key, cancellationToken);
                _logger?.LogDebug("Distributed cache removed for key: {Key}", key);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error removing value from distributed cache for key: {Key}", key);
            }
        }

        public async Task RemoveByPatternAsync(string pattern, CancellationToken cancellationToken = default)
        {
            // Redis-specific implementation would be needed for pattern-based removal
            _logger?.LogWarning("Pattern-based cache removal requires Redis-specific implementation: {Pattern}", pattern);
        }

        public async Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default)
        {
            try
            {
                var value = await _distributedCache.GetStringAsync(key, cancellationToken);
                return !string.IsNullOrEmpty(value);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error checking existence in distributed cache for key: {Key}", key);
                return false;
            }
        }
    }
}
