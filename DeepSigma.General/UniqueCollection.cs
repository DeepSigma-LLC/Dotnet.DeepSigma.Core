using DeepSigma.Core.Extensions;
using System.Collections;

namespace DeepSigma.Core;

/// <summary>
/// A collection that ensures all items are unique.
/// If a duplicate item is added, an <see cref="InvalidOperationException"/> is thrown.
/// </summary>
/// <typeparam name="T"></typeparam>
public class UniqueCollection<T> : IEnumerable<T>
{
    private HashSet<T> Collection = [];

    /// <summary>
    /// Adds an item to the collection, ensuring uniqueness.
    /// </summary>
    /// <remarks>
    /// Note: This method throws an exception (<see cref="InvalidOperationException"/>) if any item already exists in the collection.
    /// </remarks>
    /// <param name="item"></param>
    /// <exception cref="InvalidOperationException"></exception>
    public void Add(T item)
    {
        if (!Collection.Add(item)) throw new InvalidOperationException($"Duplicate value: {item}");
    }

    /// <summary>
    /// Adds multiple items to the collection, ensuring uniqueness.
    /// </summary>
    /// <remarks>
    /// Note: This method throws an exception (<see cref="InvalidOperationException"/>) if any item already exists in the collection.
    /// </remarks>
    /// <param name="items"></param>
    /// <exception cref="InvalidOperationException"></exception>
    public void UnionWith(IEnumerable<T> items)
    {
        items.ForEach(item => Add(item));
    }

    /// <summary>
    /// Clears all items from the collection.
    /// </summary>
    public void Clear() => Collection.Clear();

    /// <summary>
    /// Removes the specified item from the collection.
    /// </summary>
    /// <param name="item"></param>
    public void Remove(T item) => Collection.Remove(item);

    /// <summary>
    /// Removes all items that match the given predicate.
    /// </summary>
    /// <param name="predicate"></param>
    public void RemoveWhere(Func<T, bool> predicate)
    {
        List<T> items_to_remove = Collection.Where(predicate).ToList();
        items_to_remove.ForEach(x => Collection.Remove(x));
    }

    /// <inheritdoc/>
    public IEnumerator<T> GetEnumerator() => Collection.GetEnumerator();
    

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    
}
