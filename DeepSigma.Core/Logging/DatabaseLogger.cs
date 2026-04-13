using Microsoft.Extensions.Logging;
using DeepSigma.Core.Extensions;

namespace DeepSigma.Core.Logging;

/// <summary>
/// A logger that writes log messages to a database.
/// </summary>
[ProviderAlias("DatabaseLogger")]
public class DatabaseLogger : ILogger
{

    /// <summary>
    /// The provider that created this logger.
    /// </summary>
    protected readonly DatabaseLoggerProvider _provider;

    /// <summary>
    /// Initializes a instance of the class.
    /// </summary>
    /// <param name="provider"></param>
    public DatabaseLogger(DatabaseLoggerProvider provider)
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
        if (!IsEnabled(logLevel))
        {
            return;
        }
        LogCollection log = LogUtilities.GetLog(logLevel, eventId, state, exception);
        string json = log.ToJSON();

        if (_provider.Options.LogToDatabase is null)
        {
            throw new NotImplementedException("The log to database logic has not yet been set.");
        }
        _provider.Options.LogToDatabase(log);
    }
}
