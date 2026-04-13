
namespace DeepSigma.Core.Enums;

/// <summary>
/// Enumeration for different types of rounding operations.
/// </summary>
public enum RoundType
{
    /// <summary>
    /// Normal rounding, equivalent to Math.Round.
    /// </summary>
    Normal,
    /// <summary>
    /// Rounds down to the nearest whole number.
    /// </summary>
    RoundDown,
    /// <summary>
    /// Rounds up to the nearest whole number.
    /// </summary>
    RoundUp,
    /// <summary>
    /// Rounds towards zero, effectively truncating the decimal part.
    /// </summary>
    RoundTowardZero,
    /// <summary>
    /// Rounds away from zero, effectively rounding up for positive numbers and down for negative numbers.
    /// </summary>
    RoundAwayFromZero
}