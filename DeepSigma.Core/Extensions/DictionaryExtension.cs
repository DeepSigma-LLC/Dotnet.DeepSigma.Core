using DeepSigma.Core.Utilities;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace DeepSigma.Core.Extensions;

/// <summary>
/// Extension methods for Dictionary operations.
/// </summary>
public static class DictionaryExtension
{
    /// <summary>
    /// Gets the first key in the dictionary.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="dict"></param>
    /// <returns></returns>
    public static TKey? FirstKey<TKey, TValue>(this IDictionary<TKey, TValue> dict)
        where TKey : notnull
    {
        return dict.Keys.FirstOrDefault();
    }

    /// <summary>
    /// Gets the last key in the dictionary.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="dict"></param>
    /// <returns></returns>
    public static TKey? LastKey<TKey, TValue>(this IDictionary<TKey, TValue> dict)
        where TKey : notnull
    {
        return dict.Keys.LastOrDefault();
    }

    /// <summary>
    /// Gets the first value in the dictionary.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="dict"></param>
    /// <returns></returns>
    public static TValue? FirstValue<TKey, TValue>(this IDictionary<TKey, TValue> dict)
        where TKey : notnull
    {
        return dict.Values.FirstOrDefault();
    }

    /// <summary>
    /// Gets the last value in the dictionary.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="dict"></param>
    /// <returns></returns>
    public static TValue? LastValue<TKey, TValue>(this IDictionary<TKey, TValue> dict)
    where TKey : notnull
    {
        return dict.Values.LastOrDefault();
    }

    /// <summary>
    /// Converts a Dictionary to a SortedDictionary.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="Z"></typeparam>
    /// <param name="dictionary"></param>
    /// <returns></returns>
    public static SortedDictionary<T, Z> ToSortedDictionary<T, Z>(this Dictionary<T, Z> dictionary) 
        where T : notnull
    {
        return new SortedDictionary<T, Z>(dictionary);
    }

    /// <summary>
    /// Removes entries with null values from the dictionary.
    /// </summary>
    /// <typeparam name="K"></typeparam>
    /// <typeparam name="V"></typeparam>
    /// <param name="dictionary"></param>
    /// <returns></returns>
    public static Dictionary<K, V> RemoveNulls<K, V>(this IDictionary<K, V> dictionary)
    where K : notnull
    {
        return dictionary.Where(kvp => kvp.Value is not null)
                          .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
    }

    /// <summary>
    /// Retrieves a single series of data from the data set based on a specified target property.
    /// For example, if <typeparamref name="TDataModel"/> has a property named "Price", you can retrieve a series of prices by passing an expression that points to that property.
    /// 
    /// <code>
    /// Dictionary&lt;DateTime, decimal?&gt; priceSeries = dataSet.GetExtractedPropertyAsSeries(x => x.Price);
    /// // Or for a sorted series:
    /// SortedDictionary&lt;DateTime, decimal?&gt; priceSeries = dataSet.GetExtractedPropertyAsSeriesSorted(x => x.Price);
    /// </code>
    /// </summary>
    /// <remarks>
    /// Note: This method uses expressions to specify the target property, allowing for strong typing and compile-time checking of property names.
    /// This method will create a new Dictionary containing the keys from the original data set and the values obtained by evaluating the target property on each <typeparamref name="TDataModel"/> instance.
    /// </remarks>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TDataModel"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="data"></param>
    /// <param name="target_property"></param>
    /// <returns></returns>
    public static Dictionary<TKey, TResult> GetExtractedPropertyAsSeries<TKey, TDataModel, TResult>(this IDictionary<TKey, TDataModel> data, Expression<Func<TDataModel, TResult>> target_property)
    where TKey : notnull, IComparable<TKey>
    {
        Func<TDataModel, TResult> compiled_function = ObjectUtilities.ExpressionToExecutableFunction(target_property);

        return data.ToDictionary(
            kvp => kvp.Key,
            kvp => compiled_function(kvp.Value));
    }

    /// <summary>
    /// Tries to get a value from the dictionary.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="dict"></param>
    /// <param name="key"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static (bool found, TValue? value) TryGet<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key)
    {
        ArgumentNullException.ThrowIfNull(dict);
        return dict.TryGetValue(key, out var value) ? (true, value) : (false, default);
    }

    /// <summary>
    /// Increments the integer value associated with the specified key in the dictionary.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <param name="dict"></param>
    /// <param name="key"></param>
    /// <param name="amount"></param>
    public static void Increment<TKey>(this IDictionary<TKey, int> dict, TKey key, int amount = 1)
    {
        if (dict.ContainsKey(key))
        {
            dict[key] += amount; 
            return;
        }         
        dict[key] = amount;
    }

    /// <summary>
    /// Converts the dictionary to a readable string format.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="dict"></param>
    /// <returns></returns>
    public static string ToReadableString<TKey, TValue>(this IDictionary<TKey, TValue> dict)
    {
        return "{" + string.Join(", ", dict.Select(kvp => $"{kvp.Key}: {kvp.Value}")) + "}";
    }

    /// <summary>
    /// Gets the value associated with the specified key, or adds the key with the provided value if it does not exist.
    /// </summary>
    /// <remarks>
    /// This method uses a hyper-optimized approach with CollectionsMarshal and ref returns for performance.
    /// The implementation improves performance by reducing overhead associated with traditional dictionary access methods.
    /// </remarks>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="dict"></param>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static TValue? GetOrAdd<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, TValue? value)
        where TKey : notnull
    {
        ref var val = ref CollectionsMarshal.GetValueRefOrAddDefault(dict, key, out var exists);
        if(exists) return val;
        val = value;
        return val;
    }

    /// <summary>
    /// Tries to update the value associated with the specified key in the dictionary. 
    /// Returns true if the update was successful, false if the key does not exist.
    /// </summary>
    /// <remarks>
    /// This method uses a hyper-optimized approach with CollectionsMarshal and ref returns for performance.
    /// The implementation improves performance by reducing overhead associated with traditional dictionary access methods.
    /// </remarks>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="dict"></param>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static bool TryUpdate<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, TValue value)
        where TKey : notnull
    {
        ref var val = ref CollectionsMarshal.GetValueRefOrNullRef(dict, key);
        if (Unsafe.IsNullRef(ref val)) return false;
        val = value;
        return true;
    }
}
