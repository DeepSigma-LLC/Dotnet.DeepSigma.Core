
namespace DeepSigma.Core.Extensions;

/// <summary>
/// Extension methods for DateOnly type
/// </summary>
public static class DateOnlyExtension
{

    extension(DateTime)
    {
        /// <summary>
        /// Calculates the number of days between two dates using the 360-day year convention (30-day months).
        /// </summary>
        /// <param name="startDate">The start date.</param>
        /// <param name="endDate">The end date.</param>
        /// <param name="methodEu">True for European method, false for US (NASD) method.</param>
        /// <returns>The number of days between the two dates based on a 360-day year.</returns>
        public static int Days360(DateOnly startDate, DateOnly endDate, bool methodEu = false)
        {
            int startDay = startDate.Day;
            int startMonth = startDate.Month;
            int startYear = startDate.Year;

            int endDay = endDate.Day;
            int endMonth = endDate.Month;
            int endYear = endDate.Year;

            // Apply rules for the start date
            if (startDay == 31 || (!methodEu && IsLastDayOfFebruary(startDate)))
            {
                startDay = 30;
            }

            // Apply rules for the end date
            if (endDay == 31)
            {
                if (!methodEu && startDay != 30)
                {
                    endDay = 1;
                    if (endMonth == 12)
                    {
                        endYear++;
                        endMonth = 1;
                    }
                    else
                    {
                        endMonth++;
                    }
                }
                else
                {
                    endDay = 30;
                }
            }

            // Calculate total days
            return (endYear * 360 + endMonth * 30 + endDay) - (startYear * 360 + startMonth * 30 + startDay);
        }
    }

    /// <summary>
    /// Checks if the given date is the last day of February.
    /// </summary>
    /// <param name="date"></param>
    /// <returns></returns>
    private static bool IsLastDayOfFebruary(DateOnly date)
    {
        if (date.Month != 2) return false;
        int last_day_of_february = DateTime.DaysInMonth(date.Year, 2);
        return date.Day == last_day_of_february;
    }


    /// <summary>
    /// Converts a DateOnly to a DateTime at midnight.
    /// </summary>
    /// <param name="date_only"></param>
    /// <returns></returns>
    public static DateTime ToDateTime(this DateOnly date_only) => date_only.ToDateTime(TimeOnly.MinValue);

    /// <summary>
    /// Checks if the date is a weekday (Monday to Friday).
    /// </summary>
    /// <param name="date"></param>
    /// <returns></returns>
    public static bool IsWeekday(this DateOnly date) => !date.IsWeekend();

    /// <summary>
    /// Checks if the date is a weekend (Saturday or Sunday).
    /// </summary>
    /// <param name="date"></param>
    /// <returns></returns>
    public static bool IsWeekend(this DateOnly date) => (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday);

    /// <summary>
    /// Checks if the year of the given date is a leap year.
    /// </summary>
    /// <param name="date"></param>
    /// <returns></returns>
    public static bool IsLeapYear(this DateOnly date) => DateTime.IsLeapYear(date.Year);

    /// <summary>
    /// Returns the total number of weekdays in the year of the given date.
    /// </summary>
    /// <param name="date_time"></param>
    /// <returns></returns>
    public static int TotalWeekdaysInYear(this DateOnly date_time)
    {
        DateOnly startOfYear = new(date_time.Year, 1, 1);
        DateOnly endOfYear = new(date_time.Year, 12, 31);
        int count = 0;

        for (DateOnly date = startOfYear; date <= endOfYear; date = date.AddDays(1))
        {
            if (date.IsWeekday()) count++;
        }
        return count;
    }

    /// <summary>
    /// Returns the number of days in the month of the given date.
    /// </summary>
    /// <param name="date"></param>
    /// <returns></returns>
    public static int GetDaysInMonth(this DateOnly date) => DateTime.DaysInMonth(date.Year, date.Month);

    /// <summary>
    /// Returns the number of days in the year of the given date.
    /// </summary>
    /// <param name="date"></param>
    /// <returns></returns>
    public static int GetDaysInYear(this DateOnly date) => date.IsLeapYear() ? 366 : 365;

    /// <summary>
    /// Returns the previous weekday of the date.
    /// </summary>
    /// <param name="date"></param>
    /// <returns></returns>
    public static DateOnly PreviousWeekday(this DateOnly date) => date.AddWeekdays(-1);

    /// <summary>
    /// Returns the next weekday of the date.
    /// </summary>
    /// <param name="date"></param>
    /// <returns></returns>
    public static DateOnly NextWeekday(this DateOnly date) => date.AddWeekdays(1);

    /// <summary>
    /// Returns the date itself if it's a weekday; otherwise, returns the next weekday.
    /// </summary>
    /// <param name="date"></param>
    /// <returns></returns>
    public static DateOnly WeekdayOrNext(this DateOnly date) => date.IsWeekend() ? date.NextWeekday() : date;

    /// <summary>
    /// Returns the date itself if it's a weekday; otherwise, returns the previous weekday.
    /// </summary>
    /// <param name="date"></param>
    /// <returns></returns>
    public static DateOnly WeekdayOrPrevious(this DateOnly date) => date.IsWeekend() ? date.PreviousWeekday() : date;

    /// <summary>
    /// Returns the start of the month for the given date.
    /// </summary>
    /// <param name="date"></param>
    /// <param name="months_to_add"></param>
    /// <param name="weekday"></param>
    /// <returns></returns>
    public static DateOnly StartOfMonth(this DateOnly date, int months_to_add = 0, bool weekday = false)
    {
        return DateTimeExtension.StartOfMonth(date.ToDateTime(), months_to_add, weekday).ToDateOnly();
    }

    /// <summary>
    /// Returns the end of the month for the given date.
    /// </summary>
    /// <param name="date"></param>
    /// <param name="months_to_add"></param>
    /// <param name="weekday"></param>
    /// <returns></returns>
    public static DateOnly EndOfMonth(this DateOnly date, int months_to_add = 0, bool weekday = false)
    {
        return DateTimeExtension.EndOfMonth(date.ToDateTime(), months_to_add, weekday).ToDateOnly();
    }

    /// <summary>
    /// Adds a specified number of weekdays to the date.
    /// </summary>
    /// <param name="date"></param>
    /// <param name="Days"></param>
    /// <returns></returns>
    public static DateOnly AddWeekdays(this DateOnly date, int Days)
    {
        return DateTimeExtension.AddWeekdays(date.ToDateTime(), Days).ToDateOnly();
    }

    /// <summary>
    /// Returns 1 for the first half and 2 for the second half.
    /// </summary>
    /// <param name="Date"></param>
    /// <returns></returns>
    public static int HalfYear(this DateOnly Date) => Date.Month <= 6 ? 1 : 2;

    /// <summary>
    /// Returns the quarter of the year.
    /// </summary>
    /// <param name="Date"></param>
    /// <returns></returns>
    public static int Quarter(this DateOnly Date)
    {
        return Convert.ToInt32(Math.Ceiling((decimal)Date.Month / 3));
    }

    /// <summary>
    /// Returns the number of weekdays between two dates, excluding the start date.
    /// </summary>
    /// <param name="date"></param>
    /// <param name="Date"></param>
    /// <returns></returns>
    public static int WeekdaysFromDate(this DateOnly date, DateOnly Date) => WeekdaysBetweenDates(date, Date);

    /// <summary>
    /// Returns the number of weekdays between two dates, excluding the start date.
    /// </summary>
    /// <param name="start_date"></param>
    /// <param name="end_date"></param>
    /// <returns></returns>
    private static int WeekdaysBetweenDates(DateOnly start_date, DateOnly end_date)
    {
        int dayDifference = (int)(end_date.ToDateTime() - start_date.ToDateTime()).TotalDays;

        if (dayDifference >= 0)
        {
            return Enumerable
            .Range(1, dayDifference)
            .Select(x => start_date.AddDays(x))
            .Count(x => x.IsWeekday());
        }
        return Enumerable
            .Range(1, Math.Abs(dayDifference))
            .Select(x => start_date.AddDays(-x))
            .Count(x => x.IsWeekday());
    }

    /// <summary>
    /// Returns the start of the year for the given date, optionally ensuring it is a weekday.
    /// </summary>
    /// <param name="date"></param>
    /// <param name="weekday"></param>
    /// <returns></returns>
    public static DateOnly StartOfYear(this DateOnly date, bool weekday = false)
    {
        return DateTimeExtension.StartOfYear(date.ToDateTime(), weekday).ToDateOnly();
    }

    /// <summary>
    /// Returns the end of the year for the given date, optionally ensuring it is a weekday.
    /// </summary>
    /// <param name="date"></param>
    /// <param name="weekday"></param>
    /// <returns></returns>
    public static DateOnly EndOfYear(this DateOnly date, bool weekday = false)
    {
        return DateTimeExtension.EndOfYear(date.ToDateTime(), weekday).ToDateOnly();
    }

    /// <summary>
    /// Returns the start of the quarter for the given date, optionally ensuring it is a weekday.
    /// </summary>
    /// <param name="date"></param>
    /// <param name="weekday"></param>
    /// <returns></returns>
    public static DateOnly StartOfQuarter(this DateOnly date, bool weekday = false)
    {
        return DateTimeExtension.StartOfQuarter(date.ToDateTime(), weekday).ToDateOnly();
    }

    /// <summary>
    /// Returns the end of the quarter for the given date, optionally ensuring it is a weekday.
    /// </summary>
    /// <param name="date"></param>
    /// <param name="weekday"></param>
    /// <returns></returns>
    public static DateOnly EndOfQuarter(this DateOnly date, bool weekday = false)
    {
        return DateTimeExtension.EndOfQuarter(date.ToDateTime(), weekday).ToDateOnly(); 
    }

    /// <summary>
    /// Returns the start of the half-year for the given date, optionally ensuring it is a weekday.
    /// </summary>
    /// <param name="date"></param>
    /// <param name="weekday"></param>
    /// <returns></returns>
    public static DateOnly StartOfHalfYear(this DateOnly date, bool weekday = false)
    {
        return DateTimeExtension.StartOfHalfYear(date.ToDateTime(), weekday).ToDateOnly();
    }

    /// <summary>
    /// Returns the end of the half-year for the given date, optionally ensuring it is a weekday.
    /// </summary>
    /// <param name="date"></param>
    /// <param name="weekday"></param>
    /// <returns></returns>
    public static DateOnly EndOfHalfYear(this DateOnly date, bool weekday = false)
    {
        return DateTimeExtension.EndOfHalfYear(date.ToDateTime(), weekday).ToDateOnly();
    }

    /// <summary>
    /// Counts the occurrences of a specific day of the week between two dates, inclusive.
    /// </summary>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <param name="day_of_week"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public static int CountDaysOfWeekBetweenDates(this DateOnly start, DateOnly end, DayOfWeek day_of_week)
    {
        return DateTimeExtension.CountDaysOfWeekBetweenDates(start.ToDateTime(), end.ToDateTime(), day_of_week);
    }

    /// <summary>
    /// Returns the next specified day of the week from the given date. If the date is already the specified day, it returns the next occurrence.
    /// </summary>
    /// <param name="date"></param>
    /// <param name="dayOfWeek"></param>
    /// <returns></returns>
    public static DateOnly NextDayOfWeekSpecified(this DateOnly date, DayOfWeek dayOfWeek)
    {
        return DateTimeExtension.NextDayOfWeekSpecified(date.ToDateTime(), dayOfWeek).ToDateOnly();
    }

    /// <summary>
    /// Returns the previous specified day of the week from the given date. If the date is already the specified day, it returns the previous occurrence.
    /// </summary>
    /// <param name="date"></param>
    /// <param name="dayOfWeek"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public static DateOnly PreviousDayOfWeekSpecified(this DateOnly date, DayOfWeek dayOfWeek)
    {
        return DateTimeExtension.PreviousDayOfWeekSpecified(date.ToDateTime(), dayOfWeek).ToDateOnly();
    }

    /// <summary>
    /// Returns the next weekend day from the given date.
    /// </summary>
    /// <param name="date"></param>
    /// <returns></returns>
    public static DateOnly NextWeekendDay(this DateOnly date) => date.AddWeekendDays(1);

    /// <summary>
    /// Returns the previous weekend day from the given date.
    /// </summary>
    /// <param name="date"></param>
    /// <returns></returns>
    public static DateOnly PreviousWeekendDay(this DateOnly date) => date.AddWeekendDays(-1);

    /// <summary>
    /// Returns the date itself if it's a weekend; otherwise, returns the previous weekend day.
    /// </summary>
    /// <param name="date"></param>
    /// <returns></returns>
    public static DateOnly WeekendOrPrevious(this DateOnly date) => date.IsWeekend() ? date : date.PreviousWeekendDay();

    /// <summary>
    /// Returns the date itself if it's a weekend; otherwise, returns the next weekend day.
    /// </summary>
    /// <param name="date"></param>
    /// <returns></returns>
    public static DateOnly WeekendOrNext(this DateOnly date) => date.IsWeekend() ? date : date.NextWeekendDay();

    /// <summary>
    /// Adds days to the date until a weekend is reached.
    /// </summary>
    /// <param name="date"></param>
    /// <param name="days"></param>
    /// <returns></returns>
    public static DateOnly AddWeekendDays(this DateOnly date, int days)
    {
        return DateTimeExtension.AddWeekendDays(date.ToDateTime(), days).ToDateOnly();
    }
}

