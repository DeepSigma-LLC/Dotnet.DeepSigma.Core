using DeepSigma.Core.Extensions;
using System.Collections;

namespace DeepSigma.Core.Caching;

/// <summary>
/// A simple local cache implementation with expiration support.
/// </summary>
/// <typeparam name="TIndex"></typeparam>
/// <typeparam name="TValue"></typeparam>
public class LocalCache<TIndex, TValue> : IEnumerable<KeyValuePair<TIndex, CacheItem<TValue>>>
    where TIndex : notnull
    where TValue : class
{
    /// <summary>
    /// The internal cache storage.
    /// </summary>
    private readonly Dictionary<TIndex, CacheItem<TValue>> cache = [];

    /// <summary>
    /// The time to live for each cached item.
    /// </summary>
    private TimeSpan TimeToLive { get; init; }

    /// <summary>
    /// The method to get the value from external source.
    /// </summary>
    private Func<TIndex, TValue>? GetValueMethod { get; set; }

    /// <summary>
    /// The method to get multiple values from external source.
    /// </summary>
    private Func<List<TIndex>, List<KeyValuePair<TIndex, TValue>>>? GetValuesMethod { get; set; }

    /// <inheritdoc cref="LocalCache{TIndex, TValue}"/>
    public LocalCache(TimeSpan time_to_live, Func<TIndex, TValue>? get_value_method = null, Func<List<TIndex>, List<KeyValuePair<TIndex, TValue>>>? get_values_method = null)
    {
        TimeToLive = time_to_live;
        GetValueMethod = get_value_method;
        GetValuesMethod = get_values_method;
    }

    /// <summary>
    /// Sets the method to retrieve the value externally. 
    /// </summary>
    /// <param name="get_value_method"></param>
    public void SetGetValueMethod(Func<TIndex, TValue> get_value_method)
    {
        GetValueMethod = get_value_method;
    }

    /// <summary>
    /// Sets the method to retrieve multiple values externally.
    /// </summary>
    /// <param name="get_values_method"></param>
    public void SetGetValuesMethod(Func<List<TIndex>, List<KeyValuePair<TIndex, TValue>>> get_values_method)
    {
        GetValuesMethod = get_values_method;
    }

    /// <summary>
    /// Adds a new item to the cache.
    /// If the item already exists, it should be updated.
    /// </summary>
    /// <param name="item_index"></param>
    /// <param name="item_value"></param>
    public void Add(TIndex item_index, TValue item_value)
    {
        cache[item_index] = new CacheItem<TValue>(item_value, TimeToLive);
    }

    /// <summary>
    /// Adds a new item to the cache.
    /// If the item already exists, it will not be updated.
    /// </summary>
    /// <param name="item_index"></param>
    /// <param name="item_value"></param>
    public void AddOnlyNotUpdate(TIndex item_index, TValue item_value)
    {
        (bool Removed, var Value) = RemoveIfExpired(item_index);
        if (Removed == false && Value is not null) return;
        cache[item_index] = new(item_value, TimeToLive);
    }

    /// <summary>
    /// Tries to get a value from the cache.
    /// If the item is expired, it is removed and null is returned.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public TValue? TryGet(TIndex index)
    {
        (bool Removed, var Value) = RemoveIfExpired(index);
        if (Removed) return null;
        return Value?.Value;
    }

    /// <summary>
    /// Tries to get multiple values from the cache.
    /// If an item is expired, it is removed and not returned.
    /// </summary>
    /// <param name="items"></param>
    /// <returns></returns>
    public IEnumerable<KeyValuePair<TIndex, TValue>> TryGetMultiple(List<TIndex> items)
    {
        foreach (TIndex index in items)
        {
            TValue? cached_value = TryGet(index);
            if (cached_value is not null)
            {
                yield return new KeyValuePair<TIndex, TValue>(index, cached_value);
            }
        }
    }

    /// <summary>
    /// Tries to get a value from the cache.
    /// It will remove expired items and attempt to refresh the item using GetValueMethod.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public TValue? TryGetWithCacheRefresh(TIndex index)
    {
        TValue? result_from_cache = TryGet(index);
        if(result_from_cache is not null) return result_from_cache;
        if (GetValueMethod is null) return null;

        TValue? value = GetValueMethod(index);
        if (value is null) return null;
        cache.GetOrAdd(index, new CacheItem<TValue>(value, TimeToLive));
        return value;
    }

    /// <summary>
    /// Tries to get multiple values from the cache. 
    /// It will remove expired items and attempt to refresh missing items using GetValuesMethod.
    /// </summary>
    /// <param name="indices"></param>
    /// <returns></returns>
    public IEnumerable<KeyValuePair<TIndex, TValue>> TryGetMultipleWithCacheRefresh(List<TIndex> indices)
    {
        List<TIndex> indices_to_fetch = [];
        foreach (TIndex index in indices)
        {
            TValue? cached_value = TryGet(index);
            if (cached_value is not null)
            {
                yield return new KeyValuePair<TIndex, TValue>(index, cached_value);
            }
            else
            {
                indices_to_fetch.Add(index);
            }
        }

        if (indices_to_fetch.Count > 0 && GetValuesMethod is not null)
        {
            List<KeyValuePair<TIndex, TValue>> fetched_values = GetValuesMethod(indices_to_fetch);
            foreach (var kvp in fetched_values)
            {
                cache[kvp.Key] = new CacheItem<TValue>(kvp.Value, TimeToLive);
                yield return kvp;
            }
        }
    }

    /// <summary>
    /// Cleans up all expired items from the cache.
    /// </summary>
    public void RemoveAllExpiredItems()
    {
        IEnumerable<TIndex> keys_to_remove = cache
            .Where(kvp => kvp.Value.IsExpired)
            .Select(kvp => kvp.Key);
        keys_to_remove.ForEach(key => cache.Remove(key));
    }

    /// <summary>
    /// Removes a specific item from the cache by its index.
    /// </summary>
    /// <param name="index"></param>
    public void Remove(TIndex index)
    {
        cache.Remove(index);
    }

    /// <summary>
    /// Clears all values from the cache.
    /// </summary>
    public void Clear()
    {
        cache.Clear();
    }

    /// <summary>
    /// Checks if the cache contains a specific item by its index.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public bool Contains(TIndex index)
    {
        (bool Removed, var Value) = RemoveIfExpired(index);
        return Removed == false && Value is not null;
    }

    /// <summary>
    /// Gets the number of items in the cache.
    /// </summary>
    public int Count => cache.Count;

    /// <inheritdoc/>
    public IEnumerator<KeyValuePair<TIndex, CacheItem<TValue>>> GetEnumerator()
    {
         return cache.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    /// <summary>
    /// Removes the item if it is expired.
    /// Note: The item is not removed if it does not exist.
    /// Also, the reference is returned for further processing if needed to improve performance by avoiding double lookups.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    private (bool Removed, CacheItem<TValue>? Value) RemoveIfExpired(TIndex index)
    {
        bool found = cache.TryGetValue(index, out CacheItem<TValue>? value);
        if (value is null) return (false, null);
        if (found && value.IsExpired)
        {
            cache.Remove(index);
            return (true, null);
        }
        return (false, value);
    }
}
