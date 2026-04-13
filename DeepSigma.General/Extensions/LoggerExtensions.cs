
using Microsoft.Extensions.Logging;

namespace DeepSigma.Core.Extensions;

/// <summary>
/// Extension methods for ILogger.
/// </summary>
public static class LoggerExtensions
{
    /// <summary>
    /// Tries to log an error message with an optional exception.
    /// Note: If the logger is null (or the error is null), no action is taken.
    /// </summary>
    /// <remarks>
    /// <code>
    /// // Usage example:
    /// log.TryToLogErrorOnlyIfException(ex, "An error occurred while processing the request. {RequestId}", requestId);
    /// </code>
    /// </remarks>
    /// <param name="Logger"></param>
    /// <param name="exception"></param>
    /// <param name="message"></param>
    /// <param name="args"></param>
    public static void TryToLogErrorOnlyIfException(this ILogger? Logger, Exception? exception, string? message, params object?[] args)
    {
        if (exception == null) return;
        Logger.TryToLogError(exception, message, args);
    }

    /// <summary>
    /// Tries to log a warning message with an optional exception.
    /// Note: If the logger is null (or the error is null), no action is taken.
    /// </summary>
    /// <remarks>
    /// <code>
    /// // Usage example:
    /// log.TryToLogWarningOnlyIfException(ex, "An error occurred while processing the request. {RequestId}", requestId);
    /// </code>
    /// </remarks>
    /// <param name="Logger"></param>
    /// <param name="exception"></param>
    /// <param name="message"></param>
    /// <param name="args"></param>
    public static void TryToLogWarningOnlyIfException(this ILogger? Logger, Exception? exception, string? message, params object?[] args)
    {
        if (exception == null) return;
        Logger.TryToLogWarning(exception, message, args);
    }

    /// <summary>
    /// Tries to log an information message with an optional exception.
    /// Note: If the logger is null (or the error is null), no action is taken.
    /// </summary>
    /// <remarks>
    /// <code>
    /// // Usage example:
    /// log.TryToLogInformationOnlyIfException(ex, "Currently on request: {RequestId}", requestId);
    /// </code>
    /// </remarks>
    /// <param name="Logger"></param>
    /// <param name="exception"></param>
    /// <param name="message"></param>
    /// <param name="args"></param>
    public static void TryToLogInformationOnlyIfException(this ILogger? Logger, Exception? exception, string? message, params object?[] args)
    {
        if (exception == null) return;
        Logger.TryToLogInformation(exception, message, args);
    }

    /// <summary>
    /// Tries to log an error message with an optional exception.
    /// Note: If the logger is null, no action is taken.
    /// </summary>
    /// <remarks>
    /// <code>
    /// // Usage example:
    /// log.TryToLogError(ex, "An error occurred while processing the request. {RequestId}", requestId);
    /// </code>
    /// </remarks>
    /// <param name="Logger"></param>
    /// <param name="message"></param>
    /// <param name="exception"></param>
    /// <param name="args"></param>
    public static void TryToLogError(this ILogger? Logger, Exception? exception, string? message, params object?[] args)
    {
        Logger?.LogError(exception, message, args);
    }

    /// <summary>
    /// Tries to log an information message with an optional exception.
    /// Note: If the logger is null, no action is taken.
    /// </summary>
    /// <remarks>
    /// <code>
    /// // Usage example:
    /// log.TryToLogInformation(ex, "Invalid request made. {RequestId}", requestId);
    /// </code>
    /// </remarks>
    /// <param name="Logger"></param>
    /// <param name="message"></param>
    /// <param name="exception"></param>
    /// <param name="args"></param>
    public static void TryToLogInformation(this ILogger? Logger, Exception? exception, string? message, params object?[] args)
    {
        if (Logger is null) return;
        if (Logger.IsEnabled(LogLevel.Information) == true) return;
        if (exception is not null)
        {
            Logger.LogInformation(exception, message ?? string.Empty, args);
            return;
        }
        Logger.LogInformation(message ?? string.Empty, args);
    }

    /// <summary>
    /// Tries to log a warning message with an optional exception.
    /// Note: If the logger is null, no action is taken.
    /// </summary>
    /// <remarks>
    /// <code>
    /// // Usage example:
    /// log.TryToLogWarning(ex, "Request properties not set. {RequestId}", requestId);
    /// </code>
    /// </remarks>
    /// <param name="Logger"></param>
    /// <param name="message"></param>
    /// <param name="exception"></param>
    /// <param name="args"></param>
    public static void TryToLogWarning(this ILogger? Logger, Exception? exception, string? message, params object?[] args)
    {
        if (Logger is null) return;
        if (Logger.IsEnabled(LogLevel.Warning) != true) return;

        if (exception is not null)
        {
            Logger.LogWarning(exception, message ?? string.Empty, args);
            return;
        }
        Logger.LogWarning(message ?? string.Empty, args);
    }
}
