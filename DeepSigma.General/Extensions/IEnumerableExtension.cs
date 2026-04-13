using Newtonsoft.Json.Linq;
using System.Numerics;

namespace DeepSigma.Core.Extensions;

/// <summary>
/// Extension methods for IEnumerable collections.
/// </summary>
public static class IEnumerableExtension
{
    /// <summary>
    /// Determines if an IEnumerable is null or empty.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <returns></returns>
    public static bool IsNullOrEmpty<T>(this IEnumerable<T>? source) => source == null || !source.Any();

    /// <summary>
    /// Returns an empty IEnumerable if the source is null; otherwise, returns the source itself.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <returns></returns>
    public static IEnumerable<T> EmptyIfNull<T>(this IEnumerable<T>? source) => source ?? [];

    /// <summary>
    /// Performs the specified action on each element of the IEnumerable and returns the original IEnumerable.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="action"></param>
    /// <returns></returns>
    public static IEnumerable<T> ForEach<T>(this IEnumerable<T> source, Action<T> action)
    {
        foreach (var item in source)
        {
            action(item); 
        }
        return source;
    }

    /// <summary>
    /// Filters out null values from an IEnumerable of nullable reference types.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <returns></returns>
    public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T?> source) where T : class => source.Where(x => x != null)!;

    /// <summary>
    /// Filters out elements that match the given predicate.
    /// </summary>
    /// <remarks>
    /// <code>
    /// // Example usage:
    /// // var numbers = new List&lt;int&gt; { 1, 2, 3, 4, 5 };
    /// // var evenNumbers = numbers.WhereNot(n => n % 2 == 0); // Result: { 1, 3, 5 }
    /// </code>
    /// </remarks>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public static IEnumerable<T> WhereNot<T>(this IEnumerable<T> source, Func<T, bool> predicate) => source.Where(x => !predicate(x));


    /// <summary>
    /// Returns distinct elements from a sequence by using a specified key selector to compare values.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    /// <param name="source"></param>
    /// <param name="keySelector"></param>
    /// <returns></returns>
    public static IEnumerable<T> DistinctBy<T, TKey>(this IEnumerable<T> source, Func<T, TKey> keySelector) => source.GroupBy(keySelector).Select(g => g.First());

    /// <summary>
    /// Determines whether no elements of a sequence satisfy a condition or if the sequence is empty.
    /// </summary>
    /// <remarks>
    /// <code>
    /// // Example usage:
    /// var numbers = new List&lt;int&gt; { 1, 2, 3 };
    /// bool noneGreaterThanThree = numbers.None(n => n > 3); // Result: true
    /// 
    /// // Empty sequence case:
    /// var emptyList = new List&lt;int&gt;();
    /// bool noneInEmpty = emptyList.None(); // Result: true
    /// </code>
    /// </remarks>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public static bool None<T>(this IEnumerable<T> source, Func<T, bool>? predicate = null) => predicate == null ? !source.Any() : !source.Any(predicate);

    /// <summary>
    /// Determines whether the IEnumerable is empty.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <returns></returns>
    public static bool Empty<T>(this IEnumerable<T> source) => !source.Any();

    /// <summary>
    /// Calculates the median of a sequence of integers.
    /// </summary>
    /// <param name="source">The sequence of integers.</param>
    /// <returns>The median value.</returns>
    public static T GetMedian<T>(this IEnumerable<T> source) where T : INumber<T>
    {
        // Convert to array and sort the copy to avoid modifying the original list
        T[] temp = source.OrderBy(x => x).ToArray();
        int count = temp.Length;

        if (count == 0)
        {
            throw new InvalidOperationException("Cannot calculate median for an empty collection.");
        }

        // If the count is odd, the median is the middle element.
        if (count % 2 != 0)
        {
            return temp[count / 2];
        }
        // If the count is even, the median is the average of the two middle elements.
        else
        {
            int midIndex1 = count / 2 - 1;
            int midIndex2 = count / 2;
            return (temp[midIndex1] + temp[midIndex2]) / T.CreateChecked(2);
        }
    }

    /// <summary>
    /// Gets the most common element from a collection.
    /// If no element is most common, such as in the case of a tie, returns default.
    /// Note: Elements must be comparable for grouping. This means that for custom types, Equals and GetHashCode should be properly overridden.
    /// Record types in C# automatically provide these implementations. Structs also provide value-based equality by default.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="data"></param>
    /// <returns></returns>
    public static T? GetMostCommonElement<T>(this IEnumerable<T> data)
    {
        ArgumentNullException.ThrowIfNull(data);

        var groups = data
            .GroupBy(x => x)
            .Select(g => new { Element = g.Key, Count = g.Count() })
            .OrderByDescending(g => g.Count)
            .ToList();

        if (groups.Count == 0)  return default;

        // Handle tie or single element
        if (groups.Count == 1 || groups[0].Count != groups[1].Count)
            return groups[0].Element;

        return default; // Tie case
    }

    /// <summary>
    /// Gets the least common element from a collection.
    /// If no element is least common, such as in the case of a tie, returns default.
    /// Note: Elements must be comparable for grouping. This means that for custom types, Equals and GetHashCode should be properly overridden.
    /// Record types in C# automatically provide these implementations. Structs also provide value-based equality by default.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="data"></param>
    /// <returns></returns>
    public static T? GetLeastCommonElement<T>(this IEnumerable<T> data)
    {
        ArgumentNullException.ThrowIfNull(data);

        var groups = data
            .GroupBy(x => x)
            .Select(g => new { Element = g.Key, Count = g.Count() })
            .OrderBy(g => g.Count) // Order ascending for least common
            .ToList();

        if (groups.Count == 0) return default;

        // Handle tie or single element
        if (groups.Count == 1 || groups[0].Count != groups[1].Count)
            return groups[0].Element;

        return default; // Tie case
    }

    /// <summary>
    /// Gets the majority element from a collection, if one exists (i.e., an element that appears more than 50% of the time).
    /// If no majority element exists, returns default.
    /// Note: Elements must be comparable for grouping. This means that for custom types, Equals and GetHashCode should be properly overridden.
    /// Record types in C# automatically provide these implementations. Structs also provide value-based equality by default.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="data"></param>
    /// <param name="majority_threshold">Optional parameter that defaults to 0.5 (aka 50%) which indicates that a majority is determined by an element that occurs > 50% of the time.</param>
    /// <returns></returns>
    public static T? GetMajorityElement<T>(this IEnumerable<T> data, double majority_threshold = 0.5)
    {
        ArgumentNullException.ThrowIfNull(data);

        List<T> list = data.ToList();

        if (list.Count == 0) return default;

        var groups = list
            .GroupBy(x => x)
            .Select(g => new { Element = g.Key, Count = g.Count() })
            .OrderByDescending(g => g.Count)
            .ToList();

        var top = groups.First();
        double percent = (double)top.Count / list.Count;

        return percent > majority_threshold ? top.Element : default;
    }

    private static readonly Random _random = new();

    /// <summary>
    /// Chooses a random element from an IEnumerable.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static T? ChooseRandomElement<T>(this IEnumerable<T> source)
    {
        ArgumentNullException.ThrowIfNull(source);

        IList<T> list = source as IList<T> ?? source.ToList();
        if (list.Count == 0) return default;

        return list[_random.Next(list.Count)];
    }

    /// <summary>
    /// Shuffles the elements of an IEnumerable into a random order.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <returns></returns>
    public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source)
    {
        var rng = new Random();
        return source.OrderBy(_ => rng.Next());
    }

    /// <summary>
    /// Converts an IEnumerable to a SortableBindingList.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="enumerable"></param>
    /// <returns></returns>
    public static SortableBindingList<T> ToSortableBindingList<T>(this IEnumerable<T> enumerable)
    {
        return new SortableBindingList<T>(enumerable.ToList());
    }

    /// <summary>
    /// Converts an IEnumerable to a comma-separated string.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="enumerable"></param>
    /// <param name="seperator"></param>
    /// <returns></returns>
    public static string ToCommaSeparatedString<T>(this IEnumerable<T> enumerable, char seperator = ',')
    {
        string demlimiter = seperator.ToString();
        return string.Join(demlimiter, enumerable);
    }

    /// <summary>
    /// Joins the elements of an IEnumerable into a single string with the specified separator.
    /// </summary>
    /// <typeparam name="TObject"></typeparam>
    /// <param name="source"></param>
    /// <param name="separator"></param>
    /// <returns></returns>
    public static string JoinAsString<TObject>(this IEnumerable<TObject> source, string separator = ", ") => string.Join(separator, source);

    /// <summary>
    /// Projects each element of an IEnumerable into a new form.
    /// </summary>
    /// <remarks>
    /// <code>
    /// // Example usage:
    /// var numbers = new List&lt;int&gt; { 1, 2, 3 };
    /// var squares = numbers.Map(n => n * n); // Result: { 1, 4, 9 }
    /// </code>
    /// </remarks>
    /// <typeparam name="TObject"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="source"></param>
    /// <param name="selector"></param>
    /// <returns></returns>
    public static IEnumerable<TResult> Map<TObject, TResult>(this IEnumerable<TObject> source, Func<TObject, TResult> selector) => source.Select(selector);

    /// <summary>
    /// Projects each element of an IEnumerable into a tuple containing the element and its index.
    /// </summary>
    /// <typeparam name="TObject"></typeparam>
    /// <param name="source"></param>
    /// <returns></returns>
    public static IEnumerable<(TObject item, int index)> WithIndex<TObject>(this IEnumerable<TObject> source) => source.Select((item, index) => (item, index));

    /// <summary>
    /// Partitions an IEnumerable into two collections based on a predicate.
    /// The first collection contains elements that match the predicate, and the second contains elements that do not.
    /// </summary>
    /// <remarks>
    /// <code>
    /// // Example usage:
    /// var numbers = new List&lt;int&gt; { 1, 2, 3, 4, 5 };
    /// var (evenNumbers, oddNumbers) = numbers.Partition(n => n % 2 == 0);
    /// // evenNumbers: { 2, 4 }
    /// // oddNumbers: { 1, 3, 5 }
    /// </code>
    /// </remarks>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public static (IEnumerable<T> matches, IEnumerable<T> nonMatches) Partition<T>(this IEnumerable<T> source, Func<T, bool> predicate)
    {
        List<T> matches = [];
        List<T> nonMatches = [];

        foreach (var item in source)
        {
            if (predicate(item))
            {
                matches.Add(item);
            }
            else
            {
                nonMatches.Add(item);
            }
        }
        return (matches, nonMatches);
    }

    /// <summary>
    /// Groups the elements of an IEnumerable according to a specified key selector function.
    /// Handles null source by returning an empty collection.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    /// <param name="source"></param>
    /// <param name="keySelector"></param>
    /// <returns></returns>
    public static IEnumerable<IGrouping<TKey, T>> GroupBySafe<T, TKey>(
    this IEnumerable<T> source, Func<T, TKey> keySelector)
    {
        return source?.GroupBy(keySelector) ?? [];
    }


    /// <summary>
    /// Attempts to perform the specified action on each element of the IEnumerable, swallowing any exceptions.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="action"></param>
    /// <param name="on_exception">On exception method.</param>
    /// <returns></returns>
    public static IEnumerable<T> TryAction<T>(this IEnumerable<T> source, Action<T> action, Action<Exception>? on_exception = null)
    {
        foreach (T item in source)
        {
            try { action(item); }
            catch (Exception ex)
            {
                if (on_exception != null) on_exception(ex);
            }
        }
        return source;
    }

    /// <summary>
    /// Flattens a nested IEnumerable into a single IEnumerable.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static IEnumerable<T> Flatten<T>(this IEnumerable<IEnumerable<T>> source)
    {
        foreach (var sublist in source)
        {
            foreach (var item in sublist)
            {
                yield return item;
            }
        }
    }

    /// <summary>
    /// Returns the top N elements from an IEnumerable based on a specified key selector.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    /// <param name="source"></param>
    /// <param name="count"></param>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public static IEnumerable<T> Top<T, TKey>(this IEnumerable<T> source, int count, Func<T, TKey> predicate)
    {
        return source.OrderByDescending(predicate).Take(count);
    }

    /// <summary>
    /// Returns the bottom N elements from an IEnumerable based on a specified key selector.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    /// <param name="source"></param>
    /// <param name="count"></param>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public static IEnumerable<T> Bottom<T, TKey>(this IEnumerable<T> source, int count, Func<T, TKey> predicate)
    {
        return source.OrderBy(predicate).Take(count);
    }

    /// <summary>
    /// Calculates the weighted average of a collection.
    /// </summary>
    /// <remarks>
    /// <code>
    /// // Example usage:
    /// var items = new List&lt;Item&gt; { new Item { Value = 10, Weight = 2 }, new Item { Value = 20, Weight = 3 } };
    /// var weightedAvg = items.WeightedAverage(i => i.Value, i => i.Weight); // Result: 16
    /// </code>
    /// </remarks>
    /// <typeparam name="TObject"></typeparam>
    /// <param name="source"></param>
    /// <param name="valueSelector"></param>
    /// <param name="weightSelector"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static decimal WeightedAverage<TObject>(this IEnumerable<TObject> source, Func<TObject, decimal> valueSelector, Func<TObject, decimal> weightSelector)
    {
        ArgumentNullException.ThrowIfNull(source);

        decimal weightedSum = 0;
        decimal totalWeight = 0;

        foreach (TObject item in source)
        {
            decimal value = valueSelector(item);
            decimal weight = weightSelector(item);
            weightedSum += value * weight;
            totalWeight += weight;
        }

        return totalWeight == 0 ? 0 : weightedSum / totalWeight;
    }

    /// <summary>
    /// Calculates the running total of a sequence of decimal values.
    /// </summary>
    /// <remarks>
    /// <code>
    /// // Example usage:
    /// var numbers = new List&lt;decimal&gt; { 1.0m, 2.0m, 3.0m };
    /// var runningTotals = numbers.RunningTotal(n => n); // Result: { 1.0m, 3.0m, 6.0m }
    /// </code>
    /// </remarks>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="source"></param>
    /// <param name="selector"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static IEnumerable<decimal> RunningTotal<TValue>(this IEnumerable<TValue> source, Func<TValue, decimal> selector)
    {
        ArgumentNullException.ThrowIfNull(source);

        decimal sum = 0;
        foreach (TValue item in source)
        {
            sum += selector(item);
            yield return sum;
        }
    }

    /// <summary>
    /// Converts an IEnumerable of KeyValuePairs to a SortedList.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="enumerable"></param>
    /// <returns></returns>
    public static SortedList<TKey, TValue> ToSortedList<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> enumerable) where TKey : notnull
    {
        SortedList<TKey, TValue> list = [];
        foreach (KeyValuePair<TKey, TValue> item in enumerable)
        {
            list.Add(item.Key, item.Value);
        }
        return list;
    }

    /// <summary>
    /// Converts an IEnumerable of KeyValuePairs to a Dictionary, safely handling duplicate keys by overwriting existing entries.
    /// </summary>
    /// /// <remarks>
    /// Since existing keys are overwritten, no exception will be thrown for duplicate keys and the last occurrence encountered will be retained.
    /// </remarks>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="enumerable"></param>
    /// <returns></returns>
    public static Dictionary<TKey, TValue> ToDictionaryKeepLast<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> enumerable) where TKey : notnull
    {
        Dictionary<TKey, TValue> dict = [];
        foreach (KeyValuePair<TKey, TValue> kvp in enumerable)
        {
            dict[kvp.Key] = kvp.Value; // overwrite on duplicate keys
        }
        return dict;
    }

    /// <inheritdoc cref="ToDictionaryKeepLast{TKey, TValue}(IEnumerable{KeyValuePair{TKey, TValue}})"/>
    public static Dictionary<TKey, TValue> ToDictionaryKeepLast<TSource, TKey, TValue>(this IEnumerable<TSource> source,
        Func<TSource, TKey> keySelector, Func<TSource, TValue> valueSelector) where TKey : notnull
    {
        Dictionary<TKey, TValue> dict = [];
        foreach (TSource item in source)
        {
            TKey key = keySelector(item);
            dict[key] = valueSelector(item); // overwrite on duplicate keys
        }
        return dict;
    }

    /// <summary>
    /// Converts an IEnumerable of tuples to a Dictionary, safely handling duplicate keys by keeping the first occurrence.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="enumerable"></param>
    /// <returns></returns>
    public static Dictionary<TKey, TValue> ToDictionaryKeepFirst<TKey, TValue>(this IEnumerable<(TKey Key, TValue Value)> enumerable) where TKey : notnull
    {
        Dictionary<TKey, TValue> dict = [];
        foreach (var (Key, Value) in enumerable)
        {
            if (!dict.ContainsKey(Key))
            {
                dict[Key] = Value; // keep first occurrence
            }
        }
        return dict;
    }

    /// <inheritdoc cref="ToDictionaryKeepFirst{TKey, TValue}(IEnumerable{System.ValueTuple{TKey, TValue}})" />
    public static Dictionary<TKey, TValue> ToDictionaryKeepFirst<TSource, TKey, TValue>(this IEnumerable<TSource> source,
        Func<TSource, TKey> keySelector, Func<TSource, TValue> valueSelector) where TKey : notnull
    {
        Dictionary<TKey, TValue> dict = [];
        foreach (TSource item in source)
        {
            TKey key = keySelector(item);
            if (!dict.ContainsKey(key))
            {
                dict[key] = valueSelector(item);
            }
        }
        return dict;
    }

    /// <summary>
    /// Converts an IEnumerable of KeyValuePairs to a SortedDictionary.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="enumerable"></param>
    /// <returns></returns>
    public static SortedDictionary<TKey, TValue> ToSortedDictionary<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> enumerable) where TKey : notnull
    {
        return new SortedDictionary<TKey, TValue>(enumerable.ToDictionary(x => x.Key, x => x.Value));
    }

    /// <summary>
    /// Converts an IEnumerable of tuples to a SortedDictionary.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="enumerable"></param>
    /// <returns></returns>
    public static SortedDictionary<TKey, TValue> ToSortedDictionary<TKey, TValue>(this IEnumerable<(TKey Key, TValue Value)> enumerable) where TKey : notnull
    {
        return new SortedDictionary<TKey, TValue>(enumerable.ToDictionary(x => x.Key, x => x.Value));
    }

    /// <summary>
    /// Creates a SortedDictionary from an IEnumerable by projecting each element into a key and a value.
    /// </summary>
    /// <remarks>If the source sequence contains duplicate keys, an exception will be thrown. The resulting
    /// dictionary is sorted according to the comparer for TKey.</remarks>
    /// <typeparam name="TSource">The type of the elements in the source sequence.</typeparam>
    /// <typeparam name="TKey">The type of the keys in the resulting dictionary. Must be non-nullable.</typeparam>
    /// <typeparam name="TValue">The type of the values in the resulting dictionary.</typeparam>
    /// <param name="source">The sequence of elements to convert into a SortedDictionary.</param>
    /// <param name="keySelector">A function to extract a key from each element in the source sequence. Each key must be unique and not null.</param>
    /// <param name="valueSelector">A function to extract a value from each element in the source sequence.</param>
    /// <returns>A SortedDictionary containing keys and values projected from the source sequence.</returns>
    public static SortedDictionary<TKey, TValue> ToSortedDictionary<TSource, TKey, TValue>(this IEnumerable<TSource> source,
        Func<TSource, TKey> keySelector, Func<TSource, TValue> valueSelector) where TKey : notnull
    {
        return new SortedDictionary<TKey, TValue>(source.ToDictionary(keySelector, valueSelector));
    }

    /// <summary>
    /// Creates a SortedDictionary from an IEnumerable by projecting each element into a key and a value,
    /// Handles duplicate keys by retaining the first occurrence and ignoring subsequent ones ensuring no exception is thrown.
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="source"></param>
    /// <param name="keySelector"></param>
    /// <param name="valueSelector"></param>
    /// <returns></returns>
    public static SortedDictionary<TKey, TValue> ToSortedDictionaryKeepFirst<TSource, TKey, TValue>(this IEnumerable<TSource> source,
        Func<TSource, TKey> keySelector, Func<TSource, TValue> valueSelector) where TKey : notnull
    {
        
        return new SortedDictionary<TKey, TValue>(source.ToDictionaryKeepFirst(keySelector, valueSelector));
    }

    /// <summary>
    /// Creates a SortedDictionary from an IEnumerable by projecting each element into a key and a value,
    /// Handles duplicate keys by overwriting existing entries with the last occurrence ensuring no exception is thrown.
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="source"></param>
    /// <param name="keySelector"></param>
    /// <param name="valueSelector"></param>
    /// <returns></returns>
    public static SortedDictionary<TKey, TValue> ToSortedDictionaryKeepLast<TSource, TKey, TValue>(this IEnumerable<TSource> source,
        Func<TSource, TKey> keySelector, Func<TSource, TValue> valueSelector) where TKey : notnull
    {

        return new SortedDictionary<TKey, TValue>(source.ToDictionaryKeepLast(keySelector, valueSelector));
    }

    /// <summary>
    /// Calculates the sum of an IEnumerable of nullable decimals and formats it as a dollar value.
    /// </summary>
    /// <param name="values"></param>
    /// <param name="decimalCount"></param>
    /// <returns></returns>
    public static string? ToSumDollarValue(this IEnumerable<decimal?> values, int decimalCount = 2)
    {
        if (values.Where(x => !x.HasValue).Count() >= 1)
        {
            return String.Empty;
        }
        return values.Sum().ToStringDollarValue(decimalCount);
    }

    /// <summary>
    /// Calculates the sum of an IEnumerable of decimals and formats it as a dollar value.
    /// </summary>
    /// <param name="values"></param>
    /// <param name="decimalCount"></param>
    /// <returns></returns>
    public static string? ToSumDollarValue(this IEnumerable<decimal> values, int decimalCount = 2)
    {
        return values.Sum().ToStringDollarValue(decimalCount);
    }
}
