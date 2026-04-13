
namespace DeepSigma.Core.Logging;

/// <summary>
/// A class representing a log entry with various properties.
/// </summary>
public class LogEntry
{
    /// <summary>
    /// Log date and time in UTC.
    /// </summary>
    public DateTime LogTimeStamp { get; set; } = DateTime.UtcNow;
    /// <summary>
    /// User name of the user running the application.
    /// </summary>
    public required string UserName { get; set; } = string.Empty;
    /// <summary>
    /// User domain name of the user running the application.
    /// </summary>
    public string? UserDomainName { get; set; }
    /// <summary>
    /// Machine name where the application is running.
    /// </summary>
    public string? MachineName { get; set; }
    /// <summary>
    /// System version.
    /// </summary>
    public Version? Version { get; set; }
    /// <summary>
    /// Current directory of the application.
    /// </summary>
    public string? CurrentDirectory { get; set; }
    /// <summary>
    /// Log level Id.
    /// </summary>
    public int? LogLevelId { get; set; }
    /// <summary>
    /// Log level reason text.
    /// </summary>
    public string LogLevel { get; set; } = string.Empty;
    /// <summary>
    /// Log event id.
    /// </summary>
    public int? EventId { get; set; }
    /// <summary>
    /// Log state reason text.
    /// </summary>
    public string State { get; set; } = string.Empty;
    /// <summary>
    /// Exception message.
    /// </summary>
    public string? Exception { get; set; }
    /// <summary>
    /// Exception message.
    /// </summary>
    public string? ExceptionMessage { get; set; }
    /// <summary>
    /// Exception stack trace string.
    /// </summary>
    public string? ExceptionStackTrace { get; set; }
    /// <summary>
    /// Stack trace.
    /// </summary>
    public string? StackTrace { get; set; } = Environment.StackTrace;
    /// <summary>
    /// Environment exit code.
    /// </summary>
    public int? ExitCode { get; set; } = Environment.ExitCode;
    /// <summary>
    /// Current enviornment process count.
    /// </summary>
    public int ProcessorCount { get; set; } = Environment.ProcessorCount;

}
