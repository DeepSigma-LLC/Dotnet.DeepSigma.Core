
namespace DeepSigma.Core.Extensions;

/// <summary>
/// Extension methods for the Exception class.
/// </summary>
public static class ExceptionExtension
{
    /// <summary>
    /// Throws the exception if it is not null.
    /// </summary>
    /// <param name="exception"></param>
    public static void ThrowIfNotNull(this Exception? exception)
    {
        if (exception is not null) throw exception;
    }
}
