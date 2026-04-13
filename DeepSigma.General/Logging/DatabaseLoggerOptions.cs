
namespace DeepSigma.Core.Logging;

/// <summary>
/// Custom database logger options.
/// </summary>
public class DatabaseLoggerOptions
{
    /// <summary>
    /// Database connection string.
    /// </summary>
    public virtual required string ConnectionString { get; set; }

    /// <summary>
    /// Delegate method containing the logging logic.
    /// </summary>
    public Action<LogCollection>? LogToDatabase { get; set; }
}