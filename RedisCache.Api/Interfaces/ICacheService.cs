using Microsoft.Extensions.Caching.Distributed;

namespace RedisCache.Api.Interfaces
{
    public interface ICacheService
    {
        Task<T> GetAsync<T>(string key, CancellationToken cancellationToken = default) where T : class;

        Task<T> GetAsync<T>(string key, Func<Task<T>> factory, CancellationToken cancellationToken = default)
            where T : class;

        Task SetAsync<T>(string key, T value, CancellationToken cancellationToken, DistributedCacheEntryOptions options) 
            where T : class;

        Task RemoveAsync(string key, CancellationToken cancellationToken = default);
    }
}

