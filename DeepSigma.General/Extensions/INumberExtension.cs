using System.Numerics;

namespace DeepSigma.Core.Extensions;

/// <summary>
/// Provides extension methods for INumber types.
/// </summary>
public static class INumberExtension
{
    /// <summary>
    /// Determines whether the specified value is closer to zero than the compareTo value.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="value"></param>
    /// <param name="compareTo"></param>
    /// <returns></returns>
    public static bool IsCloserToZeroThan<T>(this T value, T compareTo)
        where T : INumber<T>
    {
        return T.Abs(value) < T.Abs(compareTo);
    }

    /// <summary>
    /// Returns the absolute value of a decimal number.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static T AbsoluteValue<T>(this T value) where T : struct, INumber<T>
    {
        return T.Abs(value);
    }

    /// <inheritdoc cref="AbsoluteValue{T}(T)"/>
    public static T? AbsoluteValue<T>(this T? value) where T : struct, INumber<T>
    {
        return value.HasValue ? T.Abs(value.Value) : null;
    }
}
