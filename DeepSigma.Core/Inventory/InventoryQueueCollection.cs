using DeepSigma.Core.Extensions;

namespace DeepSigma.Core.Inventory;

/// <summary>
/// Implementation of custom inventory collection to achieve First-In-First-Out functionality under a consistent interface to an inventory stack.
/// </summary>
/// <typeparam name="T"></typeparam>
public sealed class InventoryQueueCollection<T> : InventoryCollectionAbstract<T>
{

    /// <inheritdoc cref="InventoryQueueCollection{T}"/>
    internal InventoryQueueCollection() { }

    /// <inheritdoc/>
    public sealed override T? Peek()
    {
        return Collection.Last();
    }

    /// <inheritdoc/>
    public sealed override T? Pop()
    {
        T? item = Collection.LastOrDefault();
        if (item is null) { return default; }
        Collection.Remove(item);
        return item;
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
        Collection.AddLast(item);
    }

    /// <inheritdoc/>
    /// <param name="item"></param>
    public sealed override void AddToBack(T item)
    {
        Collection.AddFirst(item);
    }

    /// <inheritdoc/>
    /// <param name="Items"></param>
    public sealed override void Add(IEnumerable<T> Items)
    {
        Items.ForEach(item => Add(item));
    }
}
