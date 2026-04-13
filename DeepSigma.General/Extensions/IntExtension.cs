

namespace DeepSigma.Core.Extensions;

/// <summary>
/// Extension methods for int type
/// </summary>
public static class IntExtension
{
    /// <summary>
    /// Converts int to decimal
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static decimal ToDecimal(this int value) => (decimal)value;

    /// <summary>
    /// Converts nullable int to nullable decimal
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static decimal? ToDecimal(this int? value) => value.HasValue ? value.Value : null;

    /// <summary>
    /// Calculates power of int value
    /// </summary>
    /// <param name="value"></param>
    /// <param name="exponent"></param>
    /// <returns></returns>
    public static decimal Power(this int value, int exponent) => Math.Pow(value, exponent).ToDecimal();

    /// <summary>
    /// Calculates power of nullable int value
    /// </summary>
    /// <param name="value"></param>
    /// <param name="exponent"></param>
    /// <returns></returns>
    public static decimal? Power(this int? value, int exponent) => value.HasValue ? value.Value.Power(exponent) : null;

    /// <summary>
    /// Calculates square root of int value
    /// </summary>
    /// <param name="value"></param>
    /// <param name="d"></param>
    /// <returns></returns>
    public static decimal Sqrt(this int value, int d) => value * Math.Sqrt(d).ToDecimal();

    /// <summary>
    /// Calculates square root of nullable int value.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="d"></param>
    /// <returns></returns>
    public static decimal? Sqrt(this int? value, int d) => value.HasValue ? value.Value.Sqrt(d) : null;

    /// <summary>
    /// Calculates natural logarithm of int value
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static decimal Logarithm(this int value) => Math.Log(value).ToDecimal();

    /// <summary>
    /// Calculates natural logarithm of nullable int value
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static decimal? Logarithm(this int? value) => value.HasValue ? value.Value.Logarithm() : null;

    /// <summary>
    /// Determines whether the specified int is even.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static bool IsEven(this int value) => value % 2 == 0;

    /// <summary>
    /// Determines whether the specified nullable integer has a value and is an even number.
    /// </summary>
    /// <param name="value">The nullable integer to evaluate. If the value is null, the method returns false.</param>
    /// <returns>true if the value is not null and is evenly divisible by 2; otherwise, false.</returns>
    public static bool IsEven(this int? value) => value.HasValue && value.Value.IsEven();

    /// <summary>
    /// Determines whether the specified int is odd.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static bool IsOdd(this int value) => value % 2 != 0;

    /// <summary>
    /// Determines whether the specified nullable integer has a value and is an odd number.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static bool IsOdd(this int? value) => value.HasValue && value.Value.IsOdd();

}
