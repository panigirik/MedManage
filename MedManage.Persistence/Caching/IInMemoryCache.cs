namespace MedManage.Persistence.Caching;

public interface IInMemoryCache
{
    bool TryGet<T>(string key, out T? value);

    void Set<T>(string key, T value, TimeSpan? expiration = null);

    bool Remove(string key);

    bool ContainsKey(string key);

    void Clear();
}
