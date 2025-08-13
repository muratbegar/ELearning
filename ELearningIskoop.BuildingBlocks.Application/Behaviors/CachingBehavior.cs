using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using Serilog;

namespace ELearningIskoop.BuildingBlocks.Application.Behaviors
{
    // MediatR pipeline'ında Newtonsoft.Json ile caching yapmak için behavior
    // Bu sınıf, ICacheableRequest interface'ini implement eden request'leri cache'ler
    public class CachingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : notnull, ICacheableRequest
    {
        private readonly IMemoryCache _cache;
        private readonly ILogger _logger = Log.ForContext<CachingBehavior<TRequest, TResponse>>();


        // Newtonsoft.Json serializer settings
        private readonly JsonSerializerSettings _serializerSettings = new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            DateTimeZoneHandling = DateTimeZoneHandling.Utc,
            NullValueHandling = NullValueHandling.Ignore,
            Formatting = Formatting.None
        };

        public CachingBehavior(IMemoryCache cache)
        {
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            // Eğer request cache edilebilir değilse direkt işle
            if(request is not ICacheableRequest cacheableRequest)
            {
                return await next();
            }

            var cacheKey = cacheableRequest.CacheKey;
            var requestName = typeof(TRequest).Name;

            // Cache'den kontrol et
            if (_cache.TryGetValue(cacheKey, out string? cachedJson) && !string.IsNullOrEmpty(cachedJson))
            {
                try
                {
                    var cachedResponse = JsonConvert.DeserializeObject<TResponse>(cachedJson, _serializerSettings);
                    if (cachedResponse != null)
                    {
                        _logger.Information("Cache hit for {RequestName} with key {CacheKey}", requestName, cacheKey);
                        return cachedResponse;
                    }
                }
                catch (Exception ex)
                {
                    _logger.Warning(ex,
                        "Cache deserialization failed {RequestName} CacheKey: {CacheKey}",
                        requestName, cacheKey);

                    // Cache corrupted, remove it
                    _cache.Remove(cacheKey);
                }
            }
            // Cache miss - request'i işle
            _logger.Information(
                "Cache miss {RequestName} CacheKey: {CacheKey}",
                requestName, cacheKey);

            var response = await next();

            // Response'u cache'e ekle
            try
            {
                var responseJson = JsonConvert.SerializeObject(response, _serializerSettings);

                var cacheEntryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = cacheableRequest.CacheDuration,
                    Priority = CacheItemPriority.Normal,
                    Size = responseJson.Length // Memory pressure için
                };

                _cache.Set(cacheKey, responseJson, cacheEntryOptions);

                _logger.Information(
                    "Response cache'lendi {RequestName} CacheKey: {CacheKey} Süre: {Duration} Size: {Size} bytes",
                    requestName, cacheKey, cacheableRequest.CacheDuration, responseJson.Length);
            }
            catch (JsonException ex)
            {
                _logger.Warning(ex,
                    "Response cache'lenemedi {RequestName} CacheKey: {CacheKey}",
                    requestName, cacheKey);
            }
            return response;
        }
    }
}
