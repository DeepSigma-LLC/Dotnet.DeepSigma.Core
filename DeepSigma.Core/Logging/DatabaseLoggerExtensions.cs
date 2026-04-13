using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DeepSigma.Core.Logging;

/// <summary>
/// Extension methods for adding a custom database logger to the logging builder.
/// </summary>
public static class DatabaseLoggerExtensions
{
    /// <summary>
    /// Adds a custom database logger to the logging builder with configuration options.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="configure"></param>
    /// <returns></returns>
    public static ILoggingBuilder AddDatbaseLogger(this ILoggingBuilder builder, Action<DatabaseLoggerOptions> configure)
    {
        builder.Services.AddSingleton<ILoggerProvider, DatabaseLoggerProvider>();
        builder.Services.Configure(configure);
        return builder;
    }
}