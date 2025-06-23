using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using RedisCache.Api.Interfaces;

namespace RedisCache.Api.Services;

/// <summary>
/// Service for handling Redis-based caching operations using IDistributedCache.
/// Provides methods to get, set, and remove cached items, including lazy-loading support.
/// </summary>
public class RedisCacheService(IDistributedCache distributedCache) : ICacheService
{
    private static readonly JsonSerializerSettings SerializerSettings = new()
    {
        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
    };

    private static readonly DistributedCacheEntryOptions DefaultCacheOptions = new()
    {
        AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(1) // configurable if needed
    };

    /// <summary>
    /// Gets a cached item by key and deserializes it to the specified type.
    /// </summary>
    /// <typeparam name="T">Type of the object to retrieve.</typeparam>
    /// <param name="key">Cache key.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The cached item, or null if not found or deserialization fails.</returns>
    public async Task<T> GetAsync<T>(string key, CancellationToken cancellationToken = default) where T : class
    {
        var cachedValue = await distributedCache.GetStringAsync(key, cancellationToken);
        if (string.IsNullOrWhiteSpace(cachedValue))
            return null;

        try
        {
            return JsonConvert.DeserializeObject<T>(cachedValue);
        }
        catch (JsonException)
        {
            // Optionally log deserialization failure
            return null;
        }
    }

    /// <summary>
    /// Gets a cached item or uses a factory method to retrieve and cache it if missing.
    /// </summary>
    /// <typeparam name="T">Type of the object to retrieve.</typeparam>
    /// <param name="key">Cache key.</param>
    /// <param name="factory">Factory method to generate the value if cache miss occurs.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The cached or newly created item.</returns>
    public async Task<T> GetAsync<T>(string key, Func<Task<T>> factory,
        CancellationToken cancellationToken = default) where T : class
    {
        var cachedValue = await GetAsync<T>(key, cancellationToken);
        if (cachedValue is not null)
            return cachedValue;

        var value = await factory();

        if (value is not null)
        {
            await SetAsync(key, value, cancellationToken);
        }

        return value;
    }


    /// <summary>
    /// Serializes and stores an item in Redis under the specified key.
    /// </summary>
    /// <typeparam name="T">Type of the object to store.</typeparam>
    /// <param name="key">Cache key.</param>
    /// <param name="value">Object to cache.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <param name="options">Optional cache entry settings. Uses default if null.</param>
    public async Task SetAsync<T>(string key, T value, CancellationToken cancellationToken = default,
        DistributedCacheEntryOptions options = null) where T : class
    {
        if (value is null) return;

        var serialized = JsonConvert.SerializeObject(value, Formatting.None, SerializerSettings);

        await distributedCache.SetStringAsync(key, serialized, options ?? DefaultCacheOptions, cancellationToken);
    }
    

    /// <summary>
    /// Removes a cached item from Redis by its key.
    /// </summary>
    /// <param name="key">Cache key to remove.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        await distributedCache.RemoveAsync(key, cancellationToken);
    }
}