using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DeepSigma.Core.Logging;

/// <summary>
/// A provider for creating file-based loggers.
/// </summary>
[ProviderAlias("FileLogger")]
public class FileLoggerProvider : ILoggerProvider
{
    /// <summary>
    /// The options for configuring the file logger.
    /// </summary>
    public readonly FileLoggerOptions Options;

    /// <summary>
    /// Initializes a new instance of the <see cref="FileLoggerProvider"/> class with the specified options.
    /// </summary>
    /// <param name="options"></param>
    public FileLoggerProvider(IOptions<FileLoggerOptions> options)
    {
        Options = options.Value;

        if (!Directory.Exists(Options.FolderPath))
        {
            Directory.CreateDirectory(Options.FolderPath);
        }
    }

    /// <summary>
    /// Creates a logger for the specified category.
    /// </summary>
    /// <param name="categoryName"></param>
    /// <returns></returns>
    public ILogger CreateLogger(string categoryName)
    {
        return new FileLogger(this);
    }

    /// <summary>
    /// Disposes the logger provider and releases any resources.
    /// </summary>
    public void Dispose() { }
}
