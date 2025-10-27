using System.Text.Json;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace AvenSuitesApi.Application.Services.Implementations.Cache;

public class RoomAvailabilityCache
{
    private readonly IMemoryCache _cache;
    private readonly ILogger<RoomAvailabilityCache> _logger;
    private const int CACHE_EXPIRATION_MINUTES = 5;

    public RoomAvailabilityCache(
        IMemoryCache cache,
        ILogger<RoomAvailabilityCache> logger)
    {
        _cache = cache;
        _logger = logger;
    }

    public async Task<T?> GetAsync<T>(string key)
    {
        if (_cache.TryGetValue(key, out var cachedValue))
        {
            _logger.LogInformation("Cache HIT para chave: {Key}", key);
            
            if (cachedValue is T value)
                return value;
            
            if (cachedValue is string json)
                return JsonSerializer.Deserialize<T>(json);
        }

        _logger.LogInformation("Cache MISS para chave: {Key}", key);
        return default;
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null)
    {
        var options = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = expiration ?? TimeSpan.FromMinutes(CACHE_EXPIRATION_MINUTES),
            SlidingExpiration = TimeSpan.FromMinutes(2)
        };

        _cache.Set(key, value, options);
        _logger.LogInformation("Cache SET para chave: {Key}", key);
    }

    public async Task RemoveAsync(string key)
    {
        _cache.Remove(key);
        _logger.LogInformation("Cache REMOVED para chave: {Key}", key);
    }

    public string GenerateAvailabilityKey(Guid hotelId, DateTime checkIn, DateTime checkOut, Guid? roomTypeId = null)
    {
        var dateRange = $"{checkIn:yyyyMMdd}-{checkOut:yyyyMMdd}";
        var typeKey = roomTypeId?.ToString() ?? "all";
        
        return $"availability:{hotelId}:{typeKey}:{dateRange}";
    }
}

