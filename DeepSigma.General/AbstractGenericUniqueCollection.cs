using DeepSigma.Core.Extensions;
using System.Collections;

namespace DeepSigma.Core;

/// <summary>
/// An abstract base class for a generic unique collection of items of type T.
/// </summary>
/// <remarks>
/// This class uses a UniqueCollection to store items, ensuring that all items in the collection are unique.
/// </remarks>
/// <typeparam name="T"></typeparam>
public abstract class AbstractGenericUniqueCollection<T> : IEnumerable<T>
    where T : class
{
    /// <summary>
    /// The items in the collection.
    /// </summary>
    protected UniqueCollection<T> Items { get; set; } = [];

    /// <summary>
    /// The number of items in the collection.
    /// </summary>
    public int Count => Items.Count();

    /// <summary>
    /// Clears all items from the collection.
    /// </summary>
    public void Clear()
    {
        Items.Clear();
    }

    /// <summary>
    /// Adds an item to the collection.
    /// </summary>
    /// <param name="Item"></param>
    public void Add(T Item)
    {
        Items.Add(Item);
    }

    /// <summary>
    /// Adds a range of BasketPositionWeight objects to the collection.
    /// </summary>
    /// <param name="Values"></param>
    public void AddRange(IEnumerable<T> Values)
    {
        Values.ForEach(value => Items.Add(value));
    }

    /// <inheritdoc/>
    public IEnumerator<T> GetEnumerator() => Items.GetEnumerator();

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
