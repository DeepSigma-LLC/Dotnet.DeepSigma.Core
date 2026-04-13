using Microsoft.Extensions.Hosting;

namespace DeepSigma.Core.Channels;

/// <summary>
/// Provides a background service that continuously reads items from a queue and processes them using a specified
/// action.
/// </summary>
/// <remarks>
/// This service runs in the background and processes items as they become available in the queue. The
/// processing continues until the service is stopped or cancelled. However, the thread does not block while waiting for new items.
/// </remarks>
/// <typeparam name="T">The type of items to be read from the queue and processed.</typeparam>
/// <param name="queueService">The background queue service from which items are dequeued for processing. Must not be null.</param>
/// <param name="action_method">The action to perform on each dequeued item. Must not be null.</param>
public class QueueReaderService<T>(BackgroundQueueService<T> queueService, Action<T> action_method) : BackgroundService
{
    /// <inheritdoc/>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (await queueService.WaitToReadAsync(stoppingToken))
        {
            var item = await queueService.DequeueAsync(stoppingToken);
            action_method(item);
        }
    }
}