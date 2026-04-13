
namespace DeepSigma.Core.DateTimeUnification;

/// <summary>
/// Defines the interface for a custom date object.
/// </summary>
public interface IDateTime<T> :
    IComparable,
    IComparable<T>
    where T : struct, IDateTime<T>
{
    /// <summary>
    /// Implicit conversion from T to DateTime.
    /// </summary>
    /// <param name="dc"></param>
    static abstract implicit operator DateTime(T dc);

    /// <summary>
    /// Implicit conversion from T to DateOnly.
    /// </summary>
    /// <param name="dc"></param>
    static abstract implicit operator DateOnly(T dc);

    /// <summary>
    /// Implicit conversion from T to DateTimeOffset.
    /// </summary>
    /// <param name="dc"></param>
    static abstract implicit operator DateTimeOffset(T dc);

    /// <summary>
    /// Implicit conversion from DateTime to T.
    /// </summary>
    /// <param name="dc"></param>
    static abstract implicit operator T(DateTime dc);

    /// <summary>
    /// Implicit conversion from DateOnly to T.
    /// </summary>
    /// <param name="dc"></param>
    static abstract implicit operator T(DateOnly dc);

    /// <summary>
    /// Implicit conversion from DateTimeOffset to T.
    /// </summary>
    /// <param name="dc"></param>
    static abstract implicit operator T(DateTimeOffset dc);

    /// <summary>
    /// Determines whether one instance of the type is greater than another instance.
    /// </summary>
    /// <remarks>Implement this operator to define a custom greater-than comparison for the type. The behavior
    /// should be consistent with the type's comparison semantics and, if applicable, with the implementation of the
    /// less-than operator.</remarks>
    /// <param name="left">The first value to compare.</param>
    /// <param name="right">The second value to compare.</param>
    /// <returns>true if left is greater than right; otherwise, false.</returns>
    static abstract bool operator >(T left, T right);

    /// <summary>
    /// Determines whether one instance of type T is less than another.
    /// </summary>
    /// <remarks>Implement this operator to define a custom less-than comparison for the type. This operator
    /// is typically used to support ordering and sorting operations.</remarks>
    /// <param name="left">The first value to compare.</param>
    /// <param name="right">The second value to compare.</param>
    /// <returns>true if left is less than right; otherwise, false.</returns>
    static abstract bool operator <(T left, T right);

    /// <summary>
    /// Determines whether the value of the left operand is less than or equal to the value of the right operand.
    /// </summary>
    /// <param name="left">The first value to compare.</param>
    /// <param name="right">The second value to compare.</param>
    /// <returns>true if the value of left is less than or equal to the value of right; otherwise, false.</returns>
    static abstract bool operator <=(T left, T right);

    /// <summary>
    /// Determines whether the first specified value is greater than or equal to the second specified value.
    /// </summary>
    /// <param name="left">The first value to compare.</param>
    /// <param name="right">The second value to compare.</param>
    /// <returns>true if left is greater than or equal to right; otherwise, false.</returns>
    static abstract bool operator >=(T left, T right);

    /// <summary>
    /// Equality operator.
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    static abstract bool operator ==(T left, T right);

    /// <summary>
    /// Inequality operator.
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    static abstract bool operator !=(T left, T right);

    /// <summary>
    /// Subtraction operator.
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    static abstract TimeSpan operator -(T left, T right);

    /// <summary>
    /// Gets the maximum representable value.
    /// </summary>
    static abstract T MaxValue { get; }

    /// <summary>
    /// Gets the minimum representable value.
    /// </summary>
    static abstract T MinValue { get; }

    /// <inheritdoc cref="DateTime.Now"/>
    static abstract T Now { get; }

    /// <inheritdoc cref="DateTime.UtcNow"/>
    static abstract T NowUtc { get; }

    /// <summary>
    /// Gets the current date.
    /// </summary>
    static abstract T Today { get; }

    /// <summary>
    /// Creates an instance of T from a DateTime value.
    /// </summary>
    /// <param name="date"></param>
    /// <returns></returns>
    static abstract T Create(DateTime date);

    /// <summary>
    /// Creates an instance of T from a DateOnly value.
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    static abstract T Parse(string s);

    /// <inheritdoc cref="DateTime.Date"/>
    DateOnly Date { get; }

    /// <summary>
    /// Gets the DateOnly value.
    /// </summary>
    DateOnly DateOnly { get; }

    /// <summary>
    /// Gets the DateTime value.
    /// </summary>
    DateTime DateTime { get; }

    /// <summary>
    /// Gets the DateTime value.
    /// </summary>
    DateTimeOffset DateTimeOffset { get; }

    /// <inheritdoc cref="DateTime.Day"/>
    int Day { get; }

    /// <inheritdoc cref="DateTime.DayOfWeek"/>
    DayOfWeek DayOfWeek { get; }

    /// <inheritdoc cref="DateTime.DayOfYear"/>
    int DayOfYear { get; }

    /// <inheritdoc cref="DateTime.DaysInMonth(int, int)"/>
    int DaysInMonth { get; }

    /// <summary>
    /// Gets the total number of days in the year.
    /// </summary>
    int DaysInYear { get; }

    /// <inheritdoc cref="DateTime.IsLeapYear(int)"/>
    bool IsLeapYear { get; }

    /// <inheritdoc cref="DateTime.Month"/>
    int Month { get; }

    /// <inheritdoc cref="DateTime.Year"/>
    int Year { get; }

    /// <summary>
    /// Gets the quarter of the year (1 to 4).
    /// </summary>
    int Quarter { get; }

    /// <summary>
    /// Gets the half-year (1 or 2).
    /// </summary>
    int HalfYear { get; }

    /// <summary>
    /// Returns the end of the month, optionally adding months and ensuring it's a weekday.
    /// </summary>
    /// <param name="months_to_add"></param>
    /// <param name="must_be_weekday"></param>
    /// <returns></returns>
    T EndOfMonth(int months_to_add = 0, bool must_be_weekday = false);

    /// <summary>
    /// Returns the start of the month, optionally adding months and ensuring it's a weekday.
    /// </summary>
    /// <returns></returns>
    T StartOfMonth(int months_to_add = 0, bool must_be_weekday = false);

    /// <summary>
    /// Adds the specified number of weekdays to the current instance.
    /// </summary>
    /// <param name="weekdays"></param>
    /// <returns></returns>
    T AddWeekdays(int weekdays);

    /// <summary>
    /// Adds the specified number of weekend days to the current instance.
    /// </summary>
    /// <param name="weekend_days"></param>
    /// <returns></returns>
    T AddWeekendDays(int weekend_days);

    /// <summary>
    /// Determines whether the current instance falls on a weekday.
    /// </summary>
    /// <returns></returns>
    bool IsWeekday();

    /// <summary>
    /// Determines whether the current instance falls on a weekend.
    /// </summary>
    /// <returns></returns>
    bool IsWeekend();

    /// <summary>
    /// Returns the next occurrence of the specified day of the week.
    /// </summary>
    /// <param name="day_of_week"></param>
    /// <returns></returns>
    T NextDayOfWeekSpecified(DayOfWeek day_of_week);

    /// <summary>
    /// Returns the next occurrence of the specified day of the week.
    /// </summary>
    /// <param name="day_of_week"></param>
    /// <returns></returns>
    T PreviousDayOfWeekSpecified(DayOfWeek day_of_week);

    /// <inheritdoc cref="DateTime.AddDays(double)"/>
    T AddDays(int value);

    /// <inheritdoc cref="DateTime.AddMonths(int)"/>
    T AddMonths(int months);

    /// <inheritdoc cref="DateTime.AddYears(int)"/>
    T AddYears(int value);

    /// <summary>
    /// Deconstructs the current instance into its year, month, and day components.
    /// </summary>
    /// <param name="year"></param>
    /// <param name="month"></param>
    /// <param name="day"></param>
    void Deconstruct(out int year, out int month, out int day);

}