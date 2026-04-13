namespace DeepSigma.Core.Inventory;

/// <summary>
/// Defines a generic inventory collection interface with common collection operations.
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IInventoryCollection<T>
{
    /// <summary>
    /// Adds a range of elements to the inventory collection using standard collection semantics.
    /// </summary>
    /// <remarks>
    /// For instance, in a queue, this would enqueue the items in the order they are provided (the first item in the enumerable would be the first to be dequeued).
    /// In a stack, this would push the items onto the stack in the order they are provided (the last item in the enumerable would be the first to be popped).
    /// </remarks>
    /// <param name="Items"></param>
    void Add(IEnumerable<T> Items);

    /// <summary>
    /// Adds an element to the inventory collection.
    /// </summary>
    /// /// <remarks>
    /// For instance, in a queue, this would enqueue the item at the standard position (the end of the queue).
    /// In a stack, this would push the item onto the top of the stack so it can be popped next.
    /// </remarks>
    /// <param name="item"></param>
    void Add(T item);

    /// <summary>
    /// Adds an item to the top of the inventory collection so that it will be the first item retrieved regardless of its type or characteristics.
    /// </summary>
    /// <remarks>
    /// For instance, in a queue, this would place the item at the front of the queue. This means it would be the next item to be dequeued, ahead of all other items.
    /// Normally, queues do not support adding items to the front, so this method provides a way to bypass standard queue behavior.
    /// In the case of a stack, this would effectively be the same as a standard push operation since stacks always add items to the top.
    /// </remarks>
    /// <param name="item"></param>
    void AddToFront(T item);

    /// <summary>
    /// Adds an item to the end of the collection. This method provides a way to insert an item at the end of the collection regardless of its type.
    /// </summary>
    /// <remarks>
    /// For instance, in a queue, this would be the standard enqueue operation, adding the item to the back of the queue.
    /// This means it would be the last item to be dequeued after all other items have been processed.
    /// In the case of a stack, this would add the item to the bottom of the stack, which is not a standard stack operation.
    /// </remarks>
    /// <param name="item">The item to add to the back of the collection.</param>
    void AddToBack(T item);

    /// <summary>
    /// Clears all elements from the inventory collection.
    /// </summary>
    void Clear();

    /// <summary>
    /// Returns the next item in the inventory collection without removing it.
    /// </summary>
    /// <returns></returns>
    T? Peek();

    /// <summary>
    /// Removes and returns the next item from the inventory collection.
    /// </summary>
    /// <returns></returns>
    T? Pop();
}