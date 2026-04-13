
namespace DeepSigma.Core.Extensions;

/// <summary>
/// Provides extension methods for double values, including conversions to decimal.
/// </summary>
public static class DoubleExtension
{
    /// <summary>
    /// Converts a double value to a decimal.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static decimal ToDecimal(this double value) => (decimal)value;

    /// <summary>
    /// Converts a nullable double value to a nullable decimal.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static decimal? ToDecimal(this double? value) => value.HasValue ? value.Value.ToDecimal() : null;
}
