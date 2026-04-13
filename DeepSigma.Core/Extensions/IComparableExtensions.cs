
namespace DeepSigma.Core.Extensions;

/// <summary>
/// Extension methods for IComparable types.
/// This class provides methods to compare two IComparable objects easily while improving code readability when working with generic types that implement the IComparable interface.
/// These methods can be used in place of ==, >, =>, ect. operators for better clarity.
/// </summary>
public static class IComparableExtensions
{
    /// <summary>
    /// Determines if the first value is greater than the second value using the IComparable interface.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="first"></param>
    /// <param name="second"></param>
    /// <returns></returns>
    public static bool IsGreaterThan<T>(this T first, T second) where T : IComparable<T> 
    {
        return first.CompareTo(second) > 0;
    }

    /// <summary>
    /// Determines if the first value is less than the second value using the IComparable interface.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="first"></param>
    /// <param name="second"></param>
    /// <returns></returns>
    public static bool IsLessThan<T>(this T first, T second) where T : IComparable<T>
    {
        return first.CompareTo(second) < 0;
    }

    /// <summary>
    /// Determines if the first value is equal to the second value using the IComparable interface.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="first"></param>
    /// <param name="second"></param>
    /// <returns></returns>
    public static bool IsEqualTo<T>(this T first, T second) where T : IComparable<T>
    {
        return first.CompareTo(second) == 0;
    }

    /// <summary>
    /// Determines if the first value is greater than or equal to the second value using the IComparable interface.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="first"></param>
    /// <param name="second"></param>
    /// <returns></returns>
    public static bool IsGreaterThanOrEqualTo<T>(this T first, T second) where T : IComparable<T>
    {
           return first.CompareTo(second) >= 0;
    }

    /// <summary>
    /// Determines if the first value is less than or equal to the second value using the IComparable interface.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="first"></param>
    /// <param name="second"></param>
    /// <returns></returns>
    public static bool IsLessThanOrEqualTo<T>(this T first, T second) where T : IComparable<T>
    {
        return first.CompareTo(second) <= 0;
    }
}
