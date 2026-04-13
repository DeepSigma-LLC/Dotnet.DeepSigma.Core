using System.Collections.Concurrent;

namespace DeepSigma.Core.Concurrency;

/// <summary>
/// Represents the result of a method executed in a separate thread.
/// </summary>
/// <typeparam name="T"></typeparam>
public class ThreadMethodResult<T>
{
    /// <summary>
    /// Method messages logged during execution.
    /// </summary>
    public ConcurrentQueue<string> MessageLog { get; set; } = [];

    /// <summary>
    /// Results of the method execution.
    /// </summary>
    public required T MethodOutputResult { get; set; }
}
