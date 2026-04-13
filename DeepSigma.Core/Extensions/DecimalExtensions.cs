using DeepSigma.Core.Enums;
using System.Globalization;

namespace DeepSigma.Core.Extensions;

/// <summary>
/// Provides extension methods for decimal values, including rounding, formatting, and precision handling.
/// </summary>
public static class DecimalExtensions
{
    /// <summary>
    /// Converts a decimal value to a double.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static double ToDouble(this decimal value) => (double)value;

    /// <summary>
    /// Converts a nullable decimal value to a nullable double.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static double? ToDouble(this decimal? value) => value.HasValue ? value.Value.ToDouble() : null;

    /// <summary>
    /// Calculates the square root of a decimal value.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="d"></param>
    /// <returns></returns>
    public static decimal Sqrt(this decimal value, decimal d) => value * Math.Sqrt(d);

    /// <summary>
    /// Calculates the square root of a decimal value.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="d"></param>
    /// <returns></returns>
    public static decimal Sqrt(this decimal value, int d) => value * Math.Sqrt(d).ToDecimal();

    /// <summary>
    /// Calculates the square root of a nullable decimal value.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="d"></param>
    /// <returns></returns>
    public static decimal? Sqrt(this decimal? value, decimal d) => value.HasValue ? value.Value.Sqrt(d) : null;

    /// <summary>
    /// Calculates the square root of a nullable decimal value.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="d"></param>
    /// <returns></returns>
    public static decimal? Sqrt(this decimal? value, int d) => value.HasValue ? value.Value.Sqrt(d) : null;

    /// <summary>
    /// Calculates the power of a decimal value raised to the specified exponent.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="exponent"></param>
    /// <returns></returns>
    public static decimal Power(this decimal value, decimal exponent) => Math.Pow(value.ToDouble(), exponent.ToDouble()).ToDecimal();

    /// <summary>
    /// Calculates the power of a nullable decimal value raised to the specified exponent.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="exponent"></param>
    /// <returns></returns>
    public static decimal? Power(this decimal? value, decimal exponent) => value.HasValue ? (decimal?)value.Value.Power(exponent) : null;

    /// <summary>
    /// Calculates the power of a decimal value raised to the specified integer exponent exactly.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="exponent"></param>
    /// <returns></returns>
    public static decimal PowerExact(this decimal value, int exponent)
    {
        if (exponent == 0) return 1m;
        if (exponent < 0) return 1m / PowerExact(value, -exponent);

        decimal result = 1m;
        for (int i = 0; i < exponent; i++)
        {
            result *= value;
        }
        return result;
    }

    /// <summary>
    /// Calculates the power of a decimal value raised to the specified integer exponent exactly.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="exponent"></param>
    /// <returns></returns>
    public static decimal? PowerExact(this decimal? value, int exponent) => value.HasValue ? PowerExact(value.Value, exponent) : null;

    /// <summary>
    /// Calculates the complement of a decimal value (i.e., 1 - value).
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static decimal Complement(this decimal value) => 1 - value;

    /// <summary>
    /// Calculates the complement of a nullable decimal value (i.e., 1 - value).
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static decimal? Complement(this decimal? value) => value.HasValue ? value.Value.Complement() : null;

    /// <summary>
    /// Determines if a decimal value is a whole number (i.e., has no fractional part).
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static bool IsWholeNumber(this decimal value) => value % 1 == 0;

    /// <summary>
    /// Determines if a nullable decimal value is a whole number (i.e., has no fractional part).
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static bool IsWholeNumber(this decimal? value) => value.HasValue ? value.Value.IsWholeNumber() : false;

    /// <summary>
    /// Calculates the percentage of a value relative to a total.
    /// </summary>
    /// <remarks>
    /// This method returns null if the total is zero to avoid division by zero errors.
    /// It also multiplies the result by 100 to convert it to a percentage.
    /// </remarks>
    /// <param name="value"></param>
    /// <param name="total"></param>
    /// <returns></returns>
    public static decimal? PercentOf(this decimal value, decimal total) => total == 0 ? null : (value / total) * 100;


    /// <inheritdoc cref="PercentOf(decimal, decimal)"/>
    public static decimal? PercentOf(this decimal? value, decimal total) => total == 0 ? null : (value / total) * 100;

    /// <summary>
    /// Calculates the specified percentage of a decimal value.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="percent">Should be provided as a whole number (e.g., 25 for 25%)</param>
    /// <returns></returns>
    public static decimal Percent(this decimal value, decimal percent) => (value * percent) / 100;

    /// <inheritdoc cref="PercentOf(decimal, decimal)"/>
    /// <param name="value"></param>
    /// <param name="percent">Should be provided as a whole number (e.g., 25 for 25%)</param>
    public static decimal? Percent(this decimal? value, decimal percent) => value.HasValue ? value.Value.Percent(percent) : null;

    /// <summary>
    /// Adds the specified percentage to a decimal value.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="percent"></param>
    /// <returns></returns>
    public static decimal AddPercent(this decimal value, decimal percent)  => value + value.Percent(percent);

    /// <inheritdoc cref="AddPercent(decimal, decimal)"/>
    public static decimal AddPercent(this decimal? value, decimal percent) => value.HasValue ? value.Value.AddPercent(percent) : 0;

    /// <summary>
    /// Subtracts the specified percentage from a decimal value.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="percent"></param>
    /// <returns></returns>
    public static decimal SubtractPercent(this decimal value, decimal percent) => value - value.Percent(percent);

    /// <inheritdoc cref="SubtractPercent(decimal, decimal)"/>
    public static decimal? SubtractPercent(this decimal? value, decimal percent) => value.HasValue ? value.Value.SubtractPercent(percent) : 0;

    /// <summary>
    /// Rounds a decimal value based on the specified rounding type.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="roundType"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public static decimal Round(this decimal value, RoundType roundType = RoundType.Normal)
    {
        switch (roundType)
        {
            case (RoundType.Normal):
                return Math.Round(value);
            case (RoundType.RoundUp):
                return Math.Ceiling(value);
            case (RoundType.RoundDown):
                return Math.Floor(value);
            case (RoundType.RoundAwayFromZero):
                if (value > 0) { return Math.Ceiling(value); }
                else { return Math.Floor(value); }
            case (RoundType.RoundTowardZero):
                if (value > 0) { return Math.Floor(value); }
                else { return Math.Ceiling(value); }
            default:
                throw new NotImplementedException();
        }
    }

    /// <inheritdoc cref="Round(decimal, RoundType)"/>
    public static decimal? Round(this decimal? value, RoundType roundType = RoundType.Normal)
    {
        return value.HasValue ? value.Value.Round(roundType) : null;
    }

    /// <summary>
    /// Drops unnecessary precision from a decimal value.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static decimal DropUnnessaryPrecision(this decimal value)
    {
        return value / 1.000000000000000000000000000000000m;
    }

    /// <summary>
    /// Drops unnecessary precision from a decimal value.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static decimal? DropUnnessaryPrecision(this decimal? value)
    {
        return value.HasValue ? value.Value.DropUnnessaryPrecision() : null;
    }

    /// <summary>
    /// Converts a decimal value to a percentage string representation.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="decimalCount"></param>
    /// <returns></returns>
    public static string ToStringPercentage(this decimal value, int decimalCount = 3)
    {
        return value.ToString("P" + decimalCount.ToString());
    }

    /// <inheritdoc cref="ToStringPercentage(decimal, int)"/>
    public static string? ToStringPercentage(this decimal? value, int decimalCount = 3)
    {
        return value.HasValue ? value.Value.ToStringPercentage() : null;
    }

    /// <summary>
    /// Converts a decimal value to a dollar value string representation.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="decimalCount"></param>
    /// <returns></returns>
    public static string ToStringDollarValue(this decimal value, int decimalCount = 2)
    {
        return value.ToString("N" + decimalCount.ToString());
    }

    /// <inheritdoc cref="ToStringDollarValue(decimal, int)"/>
    public static string? ToStringDollarValue(this decimal? value, int decimalCount = 2)
    {
        return value.HasValue ? value.Value.ToStringDollarValue() : null;
    }

    /// <summary>
    /// Converts a decimal value to a comma-separated string representation.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static string ToStringWithCommas(this decimal value)
    {
        decimal decimalsOnly = value - Math.Truncate(value);
        string[] decimalComponents = decimalsOnly.ToString().Split('.');
        string decimalText = String.Empty;
        if (decimalComponents.Length >= 2)
        {
            decimalText = "." + decimalComponents.LastOrDefault();
        }
        return Math.Truncate(value) + decimalText;
    }

    /// <summary>
    /// Converts a nullable decimal value to a comma-separated string representation.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static string? ToStringWithCommas(this decimal? value)
    {
        return value.HasValue ? value.Value.ToStringWithCommas() : null;
    }

    /// <summary>
    /// Converts a decimal value to a currency string representation based on the specified culture.
    /// </summary>
    /// <remarks>
    /// <code>
    /// decimal amount = 1234.56m;
    /// string usCurrency = amount.ToCurrency("en-US"); // $1,234.56
    /// string ukCurrency = amount.ToCurrency("en-GB"); // £1,234.56
    /// </code>
    /// </remarks>
    /// <param name="value"></param>
    /// <param name="culture"></param>
    /// <returns></returns>
    public static string ToStringCurrency(this decimal value, string? culture = null)
    {
        var info = culture is null
            ? CultureInfo.CurrentCulture
            : new CultureInfo(culture);
        return string.Format(info, "{0:C}", value);
    }

    /// <inheritdoc cref="ToStringCurrency(decimal, string?)"/>
    public static string? ToStringCurrency(this decimal? value)
    {
        return value.HasValue ? value.Value.ToStringCurrency() : null;
    }
}