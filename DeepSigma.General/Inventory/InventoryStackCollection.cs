using DeepSigma.Core.Extensions;

namespace DeepSigma.Core.Inventory;

/// <summary>
/// Implementation of custom inventory collection to achieve Last-In-First-Out functionality under a consistent interface to an inventory queue.
/// </summary>
/// <typeparam name="T"></typeparam>
public sealed class InventoryStackCollection<T> : InventoryCollectionAbstract<T>
{
    /// <inheritdoc cref="InventoryStackCollection{T}"/>
    internal InventoryStackCollection() { }

    /// <inheritdoc/>
    /// <param name="Items"></param>
    public sealed override void Add(IEnumerable<T> Items)
    {
        Items.ForEach(item => Add(item));
    }

    /// <inheritdoc/>
    /// <param name="item"></param>
    public sealed override void Add(T item)
    {
        Collection.AddFirst(item);
    }

    /// <inheritdoc/>
    /// <param name="item"></param>
    public sealed override void AddToFront(T item)
    {
        Collection.AddFirst(item);
    }

    /// <inheritdoc/>
    /// <param name="item"></param>
    public sealed override void AddToBack(T item)
    {
        Collection.AddLast(item);
    }

    /// <inheritdoc/>
    /// <returns></returns>
    public sealed override T? Peek()
    {
        T? result = Collection.FirstOrDefault();
        return (result is null) ? default : result;
    }

    /// <inheritdoc/>
    /// <returns></returns>
    public sealed override T? Pop()
    {
        T? item = Collection.FirstOrDefault();
        if (item is null) return default;
        Collection.Remove(item);
        return item;
    }
}
