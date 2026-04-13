using System.Threading.Channels;

namespace DeepSigma.Core.Channels;

// youtube.com/watch?v=U_HUDcyh9-M

/// <summary>
/// A thread-safe background queue service backed by a <see cref="Channel{T}"/>.
/// Use <see cref="CreateUnbounded"/> or <see cref="CreateBounded"/> to construct an instance.
/// </summary>
/// <typeparam name="T">The type of items held in the queue.</typeparam>
public sealed class BackgroundMessageQueueService<T>
{
    private readonly Channel<T> _channel;

    private BackgroundMessageQueueService(Channel<T> channel) => _channel = channel;

    /// <summary>
    /// Creates a <see cref="BackgroundMessageQueueService{T}"/> backed by an unbounded channel.
    /// Items are never dropped and the queue grows as needed. Use this when producers
    /// must never block or lose data and memory pressure is acceptable.
    /// </summary>
    /// <param name="singleReader">
    /// <see langword="true"/> if at most one reader will be active at a time; enables optimizations.
    /// </param>
    /// <param name="singleWriter">
    /// <see langword="true"/> if at most one writer will be active at a time; enables optimizations.
    /// </param>
    public static BackgroundMessageQueueService<T> CreateUnbounded(
        bool singleReader,
        bool singleWriter)
    {
        var channel = Channel.CreateUnbounded<T>(new UnboundedChannelOptions
        {
            SingleReader = singleReader,
            SingleWriter = singleWriter,
        });

        return new BackgroundMessageQueueService<T>(channel);
    }

    /// <summary>
    /// Creates a <see cref="BackgroundMessageQueueService{T}"/> backed by a bounded channel.
    /// Use this when you need to apply backpressure or control memory usage.
    /// </summary>
    /// <param name="capacity">The maximum number of items the channel can hold.</param>
    /// <param name="fullMode">
    /// The strategy used when the channel is at capacity. Defaults to <see cref="BoundedChannelFullMode.Wait"/>.
    /// </param>
    /// <param name="singleReader">
    /// <see langword="true"/> if at most one reader will be active at a time; enables optimizations.
    /// </param>
    /// <param name="singleWriter">
    /// <see langword="true"/> if at most one writer will be active at a time; enables optimizations.
    /// </param>
    public static BackgroundMessageQueueService<T> CreateBounded(
        int capacity,
        bool singleReader,
        bool singleWriter,
        BoundedChannelFullMode fullMode
        )
    {
        var channel = Channel.CreateBounded<T>(new BoundedChannelOptions(capacity)
        {
            SingleReader = singleReader,
            SingleWriter = singleWriter,
            FullMode = fullMode,
        });

        return new BackgroundMessageQueueService<T>(channel);
    }

    /// <summary>
    /// Attempts to enqueue the specified item to the channel without blocking.
    /// </summary>
    /// <param name="item">The item to add to the channel.</param>
    /// <returns><see langword="true"/> if the item was successfully enqueued; otherwise, <see langword="false"/>.</returns>
    public bool TryEnqueue(T item) => _channel.Writer.TryWrite(item);

    /// <summary>
    /// Asynchronously removes and returns the next item from the queue.
    /// </summary>
    /// <param name="token">A cancellation token that can be used to cancel the dequeue operation.</param>
    /// <returns>A task that represents the asynchronous dequeue operation.</returns>
    public async Task<T> DequeueAsync(CancellationToken token = default)
        => await _channel.Reader.ReadAsync(token);

    /// <summary>
    /// Asynchronously waits until data is available to be read from the channel.
    /// </summary>
    /// <param name="token">A cancellation token that can be used to cancel the wait operation.</param>
    /// <returns>
    /// <see langword="true"/> if data is available; <see langword="false"/> if the channel is completed.
    /// </returns>
    public async ValueTask<bool> WaitToReadAsync(CancellationToken token = default)
        => await _channel.Reader.WaitToReadAsync(token);

    /// <summary>
    /// Gets the number of items currently available to be read from the channel, if supported.
    /// </summary>
    /// <remarks>Not all channel implementations support counting. If counting is not supported, this method
    /// returns null. The returned count may not reflect concurrent changes to the channel after the method is
    /// called.</remarks>
    /// <returns>The number of items available to be read from the channel, or null if the channel does not support counting.</returns>
    public int? Count() => _channel.Reader.CanCount ? _channel.Reader.Count : null;
}
