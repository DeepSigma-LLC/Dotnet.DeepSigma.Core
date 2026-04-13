using System.Threading.Channels;

namespace DeepSigma.Core.Channels;

/// <summary>
/// Demonstrates the use of Channels for producer-consumer scenarios.
/// </summary>
/// <remarks>
/// Channels provide a thread-safe way to pass data between producers and consumers asynchronously.
/// Channels can be bounded or unbounded, and support various strategies for handling full channels.
/// These strategies include waiting, dropping the oldest item, dropping the newest item, or dropping the new item.
/// This enables efficient and flexible communication patterns in concurrent applications in C#.
/// And are a good alternative to BlockingCollection (ConcurrentCollections) for many scenarios to improve performance and scalability.
/// Channels are a good fit for high-throughput scenarios where low latency is critical and can help avoid bottlenecks caused by locking.
/// For example, in real-time data processing, logging systems, task scheduling systems, hand-rolled API rate limiters, and producer-consumer queues.
/// </remarks>
internal static class ChannelExample
{
    /// <summary>
    /// Runs a producer-consumer example using a bounded channel.
    /// </summary>
    /// <param name="cancel_token">A cancellation token that can be used to cancel the operation.</param>
    /// <returns></returns>
    private static async Task RunJobsFromChannel(CancellationToken? cancel_token = null)
    {
        CancellationToken token = cancel_token ?? CancellationToken.None;

        // Create a bounded channel with a capacity of 3.
        // Use CreateUnbounded for an unbounded channel with no capacity limit. 
        var channel = Channel.CreateBounded<int>(new BoundedChannelOptions(capacity: 3)
        {
            FullMode = BoundedChannelFullMode.Wait // Wait when the channel is full
            // FullMode = BoundedChannelFullMode.DropOldest // Drop oldest item when full
            // FullMode = BoundedChannelFullMode.DropWrite // Drop new item when full
            // FullMode = BoundedChannelFullMode.DropNewest // Drop newest item when full
            //, AllowSynchronousContinuations = false // Prevent synchronous continuations
            //, SingleReader = true // Ensure only one reader
            //, SingleWriter = true // Ensure only one writer
        });

        // Producer task
        _ = Task.Run(async () =>
        {
            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine($"Producer: Writing {i}");
                await channel.Writer.WriteAsync(i, token);
                await Task.Delay(100); // Simulate some work
            }
            channel.Writer.Complete(); // Signal to channel that no more items will be written
        }, 
        token);

        // Consumer task
        await foreach (var item in channel.Reader.ReadAllAsync(token))
        {
            Console.WriteLine($"Consumer: Reading {item}");
            await Task.Delay(500, token); // Simulate slower consumption
        }

        Console.WriteLine("Channel processing complete.");
    }
}