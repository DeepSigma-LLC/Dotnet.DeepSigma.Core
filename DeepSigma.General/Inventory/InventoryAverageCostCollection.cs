using System.Collections;
using System.Diagnostics.CodeAnalysis;
using DeepSigma.Core.Extensions; 


namespace DeepSigma.Core.Inventory;

/// <summary>
/// Implementation of custom inventory collection to achieve Average Cost functionality under a consistent interface to an inventory collection.
/// </summary>
/// <typeparam name="T"></typeparam>
public class InventoryAverageCostCollection<T> : InventoryCollectionAbstract<T>, IInventoryCollection<T>, IEnumerable<T>
{
    /// <summary>
    /// Function to calculate the average cost of items in the collection.
    /// </summary>
    public required Func<IEnumerable<T>, T?> AverageCostFunction { get; init; }


    /// <inheritdoc cref="InventoryAverageCostCollection{T}"/>
    [SetsRequiredMembers]
    public InventoryAverageCostCollection(Func<IEnumerable<T>, T?> AverageCostFunction)
    {
        this.AverageCostFunction = AverageCostFunction;
    }

    /// <inheritdoc/>
    public sealed override void Add(T item)
    {
        Collection.AddFirst(item);
    }

    /// <inheritdoc/>
    public sealed override void Add(IEnumerable<T> Items)
    {
        Items.ForEach(Items => Add(Items));
    }

    /// <inheritdoc/>
    public override void AddToBack(T item)
    {
        Collection.AddLast(item);
    }

    /// <inheritdoc/>
    public override void AddToFront(T item)
    {
        Collection.AddFirst(item);
    }

    /// <inheritdoc/>
    public sealed override T? Peek()
    {
        return (Collection.Count != 0) 
            ? AverageCostFunction(Collection)
            : default;
    }

    /// <inheritdoc/>
    public sealed override T? Pop()
    {
        if (Collection.Count == 0) return default;
        
        T? averageCostItem = AverageCostFunction(Collection);
        Collection.Clear();
        return averageCostItem;
    }
}
