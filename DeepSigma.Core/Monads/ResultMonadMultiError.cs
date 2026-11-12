
namespace DeepSigma.Core.Monads;

/// <summary>
/// Generic <see cref="ResultMonad{T}"/> type that encapsulates the result of an operation.
///
/// Usage example:
/// <code>
/// List&lt;Exception&gt;? errors = DoSomething();
/// ResultMonadMultiError&lt;T&gt; result = errors is null
///    ? new SuccessResult&lt;T&gt;(default)
///    : new ErrorResults(errors);
/// string value = result switch
/// {
///     SuccessResult&lt;T&gt; success => $"Success: {success.Result}",
///     ErrorResults err => $"Errors: {string.Join(", ", err.Exceptions.Select(e => e.FriendlyMessage))}"
/// };
/// </code>
/// </summary>
/// <typeparam name="T">The wrapped success value type.</typeparam>
public readonly union ResultMonadMultiError<T>(SuccessResult<T>, ErrorResults)
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ResultMonadMultiError{T}"/> class with a successful result.
    /// </summary>
    /// <param name="success"></param>
    public static implicit operator ResultMonadMultiError<T>(SuccessResult<T> success) => new(success);

    /// <summary>
    /// Initializes a new instance of the <see cref="ResultMonadMultiError{T}"/> class with errors.
    /// </summary>
    /// <param name="errors"></param>
    public static implicit operator ResultMonadMultiError<T>(ErrorResults errors) => new(errors);
}
