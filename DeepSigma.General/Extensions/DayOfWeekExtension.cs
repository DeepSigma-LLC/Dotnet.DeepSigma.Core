using System;
using System.Collections.Generic;
using System.Text;

namespace DeepSigma.Core.Extensions;

/// <summary>
/// Extension methods for DayOfWeek enum.
/// </summary>
public static class DayOfWeekExtension
{
    /// <summary>
    /// Determines if the given day is a weekend.
    /// </summary>
    /// <param name="day"></param>
    /// <returns></returns>
    public static bool IsWeekend(this DayOfWeek day) => day == DayOfWeek.Saturday || day == DayOfWeek.Sunday;

    /// <summary>
    /// Determines if the given nullable day is a weekend.
    /// </summary>
    /// <param name="day"></param>
    /// <returns></returns>
    public static bool IsWeekend(this DayOfWeek? day) => day.HasValue && day.Value.IsWeekend();

    /// <summary>
    /// Determines if the given day is a weekday.
    /// </summary>
    /// <param name="day"></param>
    /// <returns></returns>
    public static bool IsWeekday(this DayOfWeek day) => !IsWeekend(day);

    /// <summary>
    /// Determines if the given nullable day is a weekday.
    /// </summary>
    /// <param name="day"></param>
    /// <returns></returns>
    public static bool IsWeekday(this DayOfWeek? day) => day.HasValue && day.Value.IsWeekday();
}
