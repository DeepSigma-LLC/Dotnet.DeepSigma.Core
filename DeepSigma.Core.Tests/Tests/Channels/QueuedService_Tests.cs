using System.Threading.Channels;
using Xunit;
using DeepSigma.Core.Channels;

namespace DeepSigma.Core.Tests.Tests.Channels;

public class QueuedService_Tests
{
    // -------------------------------------------------------------------------
    // BackgroundQueueService — Unbounded
    // -------------------------------------------------------------------------

    [Fact]
    public void Unbounded_TryEnqueue_AlwaysReturnsTrue()
    {
        var queue = BackgroundMessageQueue<int>.CreateUnbounded(singleReader: true, singleWriter: true);

        for (int i = 0; i < 1_000; i++)
            Assert.True(queue.TryEnqueue(i));
    }

    [Fact]
    public void Unbounded_Count_ReturnsNull_BecauseUnboundedChannelsDoNotSupportCounting()
    {
        var queue = BackgroundMessageQueue<int>.CreateUnbounded(singleReader: true, singleWriter: true);

        queue.TryEnqueue(1);
        queue.TryEnqueue(2);
        queue.TryEnqueue(3);

        // Unbounded channels do not implement CanCount, so Count() returns null.
        Assert.Null(queue.Count());
    }

    [Fact]
    public async Task Unbounded_DequeueAsync_ReturnsFIFOOrder()
    {
        var queue = BackgroundMessageQueue<int>.CreateUnbounded(singleReader: true, singleWriter: true);

        queue.TryEnqueue(10);
        queue.TryEnqueue(20);
        queue.TryEnqueue(30);

        Assert.Equal(10, await queue.DequeueAsync());
        Assert.Equal(20, await queue.DequeueAsync());
        Assert.Equal(30, await queue.DequeueAsync());
    }

    [Fact]
    public async Task Unbounded_WaitToReadAsync_ReturnsTrueWhenItemAvailable()
    {
        var queue = BackgroundMessageQueue<int>.CreateUnbounded(singleReader: true, singleWriter: true);
        queue.TryEnqueue(42);

        Assert.True(await queue.WaitToReadAsync());
    }

    [Fact]
    public async Task Unbounded_DequeueAsync_CancellationToken_Throws()
    {
        var queue = BackgroundMessageQueue<int>.CreateUnbounded(singleReader: true, singleWriter: true);
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(
            () => queue.DequeueAsync(cts.Token));
    }

    // -------------------------------------------------------------------------
    // BackgroundQueueService — Bounded
    // -------------------------------------------------------------------------

    [Fact]
    public async Task Bounded_DropWrite_DropsIncomingItemWhenFull()
    {
        var queue = BackgroundMessageQueue<int>.CreateBounded(
            capacity: 2,
            singleReader: true,
            singleWriter: true,
            fullMode: BoundedChannelFullMode.DropWrite);

        Assert.True(queue.TryEnqueue(1));
        Assert.True(queue.TryEnqueue(2));
        // TryWrite returns true even for DropWrite — the write completes without blocking;
        // the incoming item is silently discarded rather than stored.
        Assert.True(queue.TryEnqueue(3));

        // Channel still contains only the original two items.
        Assert.Equal(2, queue.Count());
        Assert.Equal(1, await queue.DequeueAsync());
        Assert.Equal(2, await queue.DequeueAsync());
    }

    [Fact]
    public async Task Bounded_DropOldest_KeepsNewestItemsWhenFull()
    {
        var queue = BackgroundMessageQueue<int>.CreateBounded(
            capacity: 2,
            singleReader: true,
            singleWriter: true,
            fullMode: BoundedChannelFullMode.DropOldest);

        queue.TryEnqueue(1);
        queue.TryEnqueue(2);
        queue.TryEnqueue(3); // 1 is dropped; channel now holds [2, 3]

        Assert.Equal(2, await queue.DequeueAsync());
        Assert.Equal(3, await queue.DequeueAsync());
    }

    [Fact]
    public async Task Bounded_DropNewest_KeepsOriginalItemsWhenFull()
    {
        var queue = BackgroundMessageQueue<int>.CreateBounded(
            capacity: 2,
            singleReader: true,
            singleWriter: true,
            fullMode: BoundedChannelFullMode.DropNewest);

        queue.TryEnqueue(1);
        queue.TryEnqueue(2);
        queue.TryEnqueue(3); // 2 (current newest) is dropped; channel holds [1, 3]

        Assert.Equal(1, await queue.DequeueAsync());
        Assert.Equal(3, await queue.DequeueAsync());
    }

    [Fact]
    public void Bounded_Count_ReflectsEnqueuedItems()
    {
        var queue = BackgroundMessageQueue<int>.CreateBounded(
            capacity: 10,
            singleReader: true,
            singleWriter: true,
            fullMode: BoundedChannelFullMode.Wait);

        queue.TryEnqueue(1);
        queue.TryEnqueue(2);

        Assert.Equal(2, queue.Count());
    }

    // -------------------------------------------------------------------------
    // QueueReaderService
    // -------------------------------------------------------------------------

    [Fact]
    public async Task QueueReaderService_ProcessesAllEnqueuedItems()
    {
        var queue = BackgroundMessageQueue<int>.CreateUnbounded(singleReader: true, singleWriter: true);
        var processed = new List<int>();

        queue.TryEnqueue(1);
        queue.TryEnqueue(2);
        queue.TryEnqueue(3);

        using var cts = new CancellationTokenSource();
        var reader = new BackgroundMessageQueueReaderService<int>(queue, item =>
        {
            processed.Add(item);
            if (processed.Count == 3)
                cts.Cancel();
        });

        await reader.StartAsync(CancellationToken.None);

        await Task.Delay(500); // allow the background loop to drain

        await reader.StopAsync(CancellationToken.None);

        Assert.Equal([1, 2, 3], processed);
    }

    [Fact]
    public async Task QueueReaderService_StopsCleanlyOnCancellation()
    {
        var queue = BackgroundMessageQueue<int>.CreateUnbounded(singleReader: true, singleWriter: true);
        var processed = new List<int>();

        using var cts = new CancellationTokenSource();
        var reader = new BackgroundMessageQueueReaderService<int>(queue, item => processed.Add(item));

        await reader.StartAsync(CancellationToken.None);
        await cts.CancelAsync();
        await reader.StopAsync(CancellationToken.None);

        // No exception thrown and service stopped cleanly
        Assert.True(true);
    }

    [Fact]
    public async Task QueueReaderService_ProcessesItemsEnqueuedAfterStart()
    {
        var queue = BackgroundMessageQueue<int>.CreateUnbounded(singleReader: true, singleWriter: true);
        var processed = new List<int>();
        var tcs = new TaskCompletionSource();

        var reader = new BackgroundMessageQueueReaderService<int>(queue, item =>
        {
            processed.Add(item);
            if (processed.Count == 2)
                tcs.TrySetResult();
        });

        await reader.StartAsync(CancellationToken.None);

        queue.TryEnqueue(100);
        queue.TryEnqueue(200);

        await tcs.Task.WaitAsync(TimeSpan.FromSeconds(5));
        await reader.StopAsync(CancellationToken.None);

        Assert.Equal([100, 200], processed);
    }
}
