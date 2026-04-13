using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DeepSigma.Core.Logging;

/// <summary>
/// A provider for creating a custom database logger.
/// </summary>
public class DatabaseLoggerProvider : ILoggerProvider
{
    /// <summary>
    /// The options for configuring the custom database logger.
    /// </summary>
    public readonly DatabaseLoggerOptions Options;

    /// <summary>
    /// Initializes a new instance of the <see cref="DatabaseLoggerProvider"/> class with the specified options.
    /// </summary>
    /// <param name="options"></param>
    public DatabaseLoggerProvider(IOptions<DatabaseLoggerOptions> options)
    {
        Options = options.Value;
    }

    /// <summary>
    /// Creates a logger for the specified category.
    /// </summary>
    /// <param name="categoryName"></param>
    /// <returns></returns>
    public ILogger CreateLogger(string categoryName)
    {
        return new DatabaseLogger(this);
    }

    /// <summary>
    /// Disposes the logger provider and releases any resources.
    /// </summary>
    public void Dispose() { }
}