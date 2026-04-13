using System.Collections;

namespace DeepSigma.Core.Inventory;

/// <summary>
/// Abstract class for an inventory collection that provides basic functionality for managing a collection of items.
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class InventoryCollectionAbstract<T> 
    : IInventoryCollection<T>, IEnumerable<T>
{
    private protected LinkedList<T> Collection { get; set; } = new LinkedList<T>();

    /// <inheritdoc/>
    public void Clear()
    {
        Collection.Clear();
    }

    /// <inheritdoc/>
    public abstract void Add(IEnumerable<T> Items);

    /// <inheritdoc/>
    /// <param name="item"></param>
    public abstract void Add(T item);

    /// <inheritdoc/>
    public abstract T? Peek();

    /// <inheritdoc/>
    public abstract T? Pop();

    /// <inheritdoc/>
    /// <param name="item"></param>
    public abstract void AddToFront(T item);

    /// <inheritdoc/>
    /// <param name="item"></param>
    public abstract void AddToBack(T item);

    /// <inheritdoc/>
    public IEnumerator<T> GetEnumerator() => Collection.GetEnumerator();

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    
}
