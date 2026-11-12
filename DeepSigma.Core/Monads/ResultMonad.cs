
namespace DeepSigma.Core.Monads;

/// <summary>
/// Generic <see cref="ResultMonad{T}"/> type that encapsulates the result of an operation.
///
/// Usage example:
/// <code>
/// Exception? error = DoSomething();
/// ResultMonad&lt;T&gt; result = error is null
///    ? new SuccessResult&lt;T&gt;(default)
///    : new ErrorResult(error);
/// string value = result switch
/// {
///     SuccessResult&lt;T&gt; success => $"Success: {success.Result}",
///     ErrorResult err => $"Error: {err.Exception.Message}"
/// };
/// </code>
/// </summary>
/// <typeparam name="T">The wrapped success value type.</typeparam>
public readonly union ResultMonad<T>(SuccessResult<T>, ErrorResult)
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ResultMonad{T}"/> class with a successful result.
    /// </summary>
    /// <param name="success"></param>
    public static implicit operator ResultMonad<T>(SuccessResult<T> success) => new(success);

    /// <summary>
    /// Initializes a new instance of the <see cref="ResultMonad{T}"/> class with an error.
    /// </summary>
    /// <param name="error"></param>
    public static implicit operator ResultMonad<T>(ErrorResult error) => new(error);
}