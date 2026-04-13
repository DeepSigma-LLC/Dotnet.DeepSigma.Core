using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DeepSigma.Core.Logging;

/// <summary>
/// Extension methods for adding a file logger to the logging builder.
/// </summary>
public static class FileLoggerExtensions
{
    /// <summary>
    /// Adds a file logger to the logging builder with configuration options.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="configure"></param>
    /// <returns></returns>
    public static ILoggingBuilder AddFileLogger(this ILoggingBuilder builder, Action<FileLoggerOptions> configure)
    {
        builder.Services.AddSingleton<ILoggerProvider, FileLoggerProvider>();
        builder.Services.Configure(configure);
        return builder;
    }
}
