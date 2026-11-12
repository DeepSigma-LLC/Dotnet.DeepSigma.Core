using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DeepSigma.Core.Channels;

/// <summary>
/// Provides a background service that continuously reads items from a queue and processes them using a specified action.
/// </summary>
/// <remarks>
/// This service runs in the background and processes items as they become available in the queue. The
/// processing continues until the service is stopped or cancelled. However, the thread does not block while waiting for new items.
/// 
/// To add this service to your application, you can register it in the dependency injection container, for example:
/// <code>
/// builder.Services.Configure&lt;HostOptions&gt;(options =>
/// {
///     options.ServicesStartConcurrently = true; // Allow hosted services to start concurrently for faster startup.
///     options.ServicesStopConcurrently = true; // Allow hosted services to stop concurrently for faster shutdown.
/// });
/// 
/// // Register the queue service as a singleton so it can be shared across the application.
/// services.AddSingleton&lt;BackgroundMessageQueue&lt;YourItemType&gt;&gt;(); 
/// // Replace YourItemType with the actual type of items in your queue.
/// services.AddHostedService&lt;BackgroundMessageQueueReaderService&lt;YourItemType&gt;&gt;();
/// </code>
/// </remarks>
/// <typeparam name="T">The type of items to be read from the queue and processed.</typeparam>
/// <param name="queueService">The background queue service from which items are dequeued for processing. Must not be null.</param>
/// <param name="action_method">The action to perform on each dequeued item. Must not be null.</param>
/// <param name="logger">An optional logger for logging information, warnings, or errors during the processing of items.</param>
public class BackgroundMessageQueueReaderService<T>(BackgroundMessageQueue<T> queueService, Action<T> action_method, ILogger<BackgroundMessageQueueReaderService<T>>? logger = null)
    : BackgroundService, IHostedLifecycleService
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

    /// <inheritdoc/>
    public Task StartingAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    /// <inheritdoc/>
    public Task StartedAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    /// <inheritdoc/>
    public Task StoppingAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    /// <inheritdoc/>
    public Task StoppedAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}