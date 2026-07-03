namespace NorthwindStore.Interfaces;

public interface ICacheService
{
    Task<T> GetOrCreateAsync<T>(string key, Func<Task<T>> factory, TimeSpan? duration = null);
    void Remove(string key);
    void InvalidateProducts();
}
