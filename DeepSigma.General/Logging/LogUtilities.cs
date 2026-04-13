using Microsoft.Extensions.Logging;

namespace DeepSigma.Core.Logging;

internal class LogUtilities
{
    /// <summary>
    /// Generates log
    /// </summary>
    /// <typeparam name="TState"></typeparam>
    /// <param name="logLevel"></param>
    /// <param name="eventId"></param>
    /// <param name="state"></param>
    /// <param name="exception"></param>
    /// <returns></returns>
    internal static LogCollection GetLog<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception)
    {
        LogEntry log = new()
        {
            MachineName = Environment.MachineName,
            UserName = Environment.UserName,
            CurrentDirectory = Environment.CurrentDirectory,
            UserDomainName = Environment.UserDomainName,
            Version = Environment.Version,
            LogLevelId = (int)logLevel,
            LogLevel = logLevel.ToString().ToUpper(),
            EventId = eventId.Id,
            State = state?.ToString() ?? string.Empty,
            Exception = exception?.ToString() ?? string.Empty,
            ExceptionMessage = exception?.Message ?? string.Empty,
            ExceptionStackTrace = exception?.StackTrace,
            StackTrace = Environment.StackTrace,
            ProcessorCount = Environment.ProcessorCount,
            ExitCode = Environment.ExitCode,
        };

        LogCollection logs = new();
        logs.Entries.Add(log);
        return logs;
    }
}