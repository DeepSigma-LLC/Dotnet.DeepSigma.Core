namespace DeepSigma.Core.Monads;

/// <summary>
/// Represents the result of an operation as a success.
/// </summary>
/// <typeparam name="T"></typeparam>
/// <param name="Result"></param>
public sealed record SuccessResult<T>(T? Result);

/// <summary>
/// Represents the result of an operation as a failure with an error.
/// </summary>
/// <param name="Exception"></param>
public sealed record ErrorResult(Exception Exception);

/// <summary>
/// Represents a collection of errors that occurred during an operation.
/// </summary>
/// <param name="Exceptions"></param>
public sealed record ErrorResults(IEnumerable<Exception> Exceptions);
