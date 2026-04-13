using Microsoft.Extensions.Logging;
using DeepSigma.Core.Extensions;

namespace DeepSigma.Core.Logging;

/// <summary>
/// A logger that writes log messages to a file.
/// </summary>
public class FileLogger : ILogger
{
    /// <summary>
    /// File logger provider.
    /// </summary>
    protected readonly FileLoggerProvider _provider;

    /// <summary>
    /// Initializes an instance of the class.
    /// </summary>
    /// <param name="provider"></param>
    public FileLogger(FileLoggerProvider provider)
    {
        _provider = provider;

    }

    /// <summary>
    /// Begins a logical operation scope.
    /// </summary>
    /// <typeparam name="TState"></typeparam>
    /// <param name="state"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public IDisposable? BeginScope<TState>(TState state) where TState : notnull
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Check if the given log level is enabled.
    /// </summary>
    /// <param name="logLevel"></param>
    /// <returns></returns>
    public bool IsEnabled(LogLevel logLevel)
    {
        return logLevel != LogLevel.None;
    }

    /// <summary>
    /// Logs a message.
    /// </summary>
    /// <typeparam name="TState"></typeparam>
    /// <param name="logLevel"></param>
    /// <param name="eventId"></param>
    /// <param name="state"></param>
    /// <param name="exception"></param>
    /// <param name="formatter"></param>
    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        if (!IsEnabled(logLevel)) { return; }

        string new_file_name = _provider.Options.FileName.Replace("{Date}", DateTime.UtcNow.ToStringFileFormat()).Replace("{Username}", Environment.UserName);
        string full_file_path = Path.Combine(_provider.Options.FolderPath, new_file_name);

        LogCollection logs = LogUtilities.GetLog(logLevel, eventId, state, exception);
        string json = logs.ToJSON();

        using var stream = new StreamWriter(full_file_path, false);
        stream.WriteLine(json);
    }
}
