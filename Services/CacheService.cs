using Microsoft.Extensions.Caching.Memory;
using NorthwindStore.Interfaces;

namespace NorthwindStore.Services;

public class CacheService(IMemoryCache cache) : ICacheService
{
    public const string AvailableProducts = "products:available";
    public const string LowStockProducts = "products:low-stock";
    public const string OutOfStockProducts = "products:out-of-stock";
    public const string DiscontinuedProducts = "products:discontinued";

    public async Task<T> GetOrCreateAsync<T>(string key, Func<Task<T>> factory, TimeSpan? duration = null)
    {
        if (cache.TryGetValue(key, out T? cached) && cached is not null)
        {
            return cached;
        }

        var value = await factory();
        cache.Set(key, value, duration ?? TimeSpan.FromMinutes(5));
        return value;
    }

    public void Remove(string key) => cache.Remove(key);

    public void InvalidateProducts()
    {
        Remove(AvailableProducts);
        Remove(LowStockProducts);
        Remove(OutOfStockProducts);
        Remove(DiscontinuedProducts);
    }
}
