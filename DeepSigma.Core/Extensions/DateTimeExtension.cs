
namespace DeepSigma.Core.Extensions;

/// <summary>
/// Extension methods for DateTime operations.
/// </summary>
public static class DateTimeExtension
{
    private static string _dateStringFormat { get; } = "yyyy-MM-dd 'T 'HH-mm-ss";

    extension(DateTime)
    {
        /// <summary>
        /// Calculates the number of days between two dates using the 360-day year convention (30-day months).
        /// </summary>
        /// <param name="startDate">The start date.</param>
        /// <param name="endDate">The end date.</param>
        /// <param name="methodEu">True for European method, false for US (NASD) method.</param>
        /// <returns>The number of days between the two dates based on a 360-day year.</returns>
        public static int Days360(DateTime startDate, DateTime endDate, bool methodEu = false)
        {
            return DateOnlyExtension.Days360(startDate.ToDateOnly(), endDate.ToDateOnly(), methodEu);
        }
    }

    /// <summary>
    /// Converts a DateTime to a DateOnly.
    /// </summary>
    /// <param name="date_time"></param>
    /// <returns></returns>
    public static DateOnly ToDateOnly(this DateTime date_time) => DateOnly.FromDateTime(date_time);

    /// <summary>
    /// Checks if the year of the given date is a leap year.
    /// </summary>
    /// <param name="date"></param>
    /// <returns></returns>
    public static bool IsLeapYear(this DateTime date) => DateTime.IsLeapYear(date.Year);

    /// <summary>
    /// Returns the number of days in the month of the given date.
    /// </summary>
    /// <param name="date"></param>
    /// <returns></returns>
    public static int GetDaysInMonth(this DateTime date) => DateTime.DaysInMonth(date.Year, date.Month);

    /// <summary>
    /// Returns the number of days in the year of the given date.
    /// </summary>
    /// <param name="date"></param>
    /// <returns></returns>
    public static int GetDaysInYear(this DateTime date) => date.IsLeapYear() ? 366 : 365;

    /// <summary>
    /// Returns the total number of weekdays (Monday to Friday) in the year of the given date.
    /// </summary>
    /// <param name="date_time"></param>
    /// <returns></returns>
    public static int TotalWeekdaysInYear(this DateTime date_time)
    {
        return DateOnlyExtension.TotalWeekdaysInYear(date_time.ToDateOnly());
    }

    /// <summary>
    /// Converts a DateTime to a string in the format "yyyy-MM-dd T HH-mm-ss".
    /// </summary>
    /// <param name="date"></param>
    /// <returns></returns>
    public static string ToStringFileFormat(this DateTime date) => date.ToString(_dateStringFormat);

    /// <summary>
    /// Returns the previous weekday of the date.
    /// </summary>
    /// <param name="date"></param>
    /// <returns></returns>
    public static DateTime PreviousWeekday(this DateTime date) => date.AddWeekdays(-1);

    /// <summary>
    /// Returns the next weekday of the date.
    /// </summary>
    /// <param name="date"></param>
    /// <returns></returns>
    public static DateTime NextWeekday(this DateTime date) => date.AddWeekdays(1);


    /// <summary>
    /// Returns the date itself if it's a weekday; otherwise, returns the next weekday.
    /// </summary>
    /// <param name="date"></param>
    /// <returns></returns>
    public static DateTime WeekdayOrNext(this DateTime date) => date.IsWeekend() ? date.NextWeekday() : date;

    /// <summary>
    /// Returns the date itself if it's a weekday; otherwise, returns the previous weekday.
    /// </summary>
    /// <param name="date"></param>
    /// <returns></returns>
    public static DateTime WeekdayOrPrevious(this DateTime date) => date.IsWeekend() ? date.PreviousWeekday() : date;

    /// <summary>
    /// Returns the next weekend day from the given date.
    /// </summary>
    /// <param name="date"></param>
    /// <returns></returns>
    public static DateTime NextWeekendDay(this DateTime date) => date.AddWeekendDays(1);

    /// <summary>
    /// Returns the previous weekend day from the given date.
    /// </summary>
    /// <param name="date"></param>
    /// <returns></returns>
    public static DateTime PreviousWeekendDay(this DateTime date) => date.AddWeekendDays(-1);

    /// <summary>
    /// Returns the date itself if it's a weekend; otherwise, returns the previous weekend day.
    /// </summary>
    /// <param name="date"></param>
    /// <returns></returns>
    public static DateTime WeekendOrPrevious(this DateTime date) => date.IsWeekend() ? date : date.PreviousWeekendDay();

    /// <summary>
    /// Returns the date itself if it's a weekend; otherwise, returns the next weekend day.
    /// </summary>
    /// <param name="date"></param>
    /// <returns></returns>
    public static DateTime WeekendOrNext(this DateTime date) => date.IsWeekend() ? date : date.NextWeekendDay();

    /// <summary>
    /// Adds a specified number of weekdays to the date.
    /// </summary>
    /// <param name="date"></param>
    /// <param name="days"></param>
    /// <returns></returns>
    public static DateTime AddWeekdays(this DateTime date, int days)
    {
        int AbsoluteValueOfDays = Math.Abs(days);
        int DaysToAdd = days / AbsoluteValueOfDays;
        for (int i = 0; i < AbsoluteValueOfDays; i++)
        {
            do
            {
                date = date.AddDays(DaysToAdd);
            }
            while (date.IsWeekend() == true);
            i++;
        }
        return date;
    }

    /// <summary>
    /// Adds a specified number of weekdays to the date.
    /// </summary>
    /// <param name="date"></param>
    /// <param name="days"></param>
    /// <returns></returns>
    public static DateTime AddWeekendDays(this DateTime date, int days)
    {
        int AbsoluteValueOfDays = Math.Abs(days);
        int DaysToAdd = days / AbsoluteValueOfDays;
        for (int i = 0; i < AbsoluteValueOfDays; i++)
        {
            do
            {
                date = date.AddDays(DaysToAdd);
            }
            while (date.IsWeekday() == true);
            i++;
        }
        return date;
    }

    /// <summary>
    /// Returns the next specified day of the week from the given date. If the date is already the specified day, it returns the next occurrence.
    /// </summary>
    /// <param name="date"></param>
    /// <param name="dayOfWeek"></param>
    /// <returns></returns>
    public static DateTime NextDayOfWeekSpecified(this DateTime date, DayOfWeek dayOfWeek)
    {
        for (int i = 0; i < 8; i++) // Stops at 8 since this will be more than enough.
        {
            date = date.AddDays(1);
            if (date.DayOfWeek == dayOfWeek) return date;
        }
        throw new Exception("Next day of week was not found. We should never hit this exception or else this logic should be reviewed.");
    }

    /// <summary>
    /// Returns the previous specified day of the week from the given date. If the date is already the specified day, it returns the previous occurrence.
    /// </summary>
    /// <param name="date"></param>
    /// <param name="dayOfWeek"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public static DateTime PreviousDayOfWeekSpecified(this DateTime date, DayOfWeek dayOfWeek)
    {
        for (int i = 0; i < 8; i++) // Stops at 8 since this will be more than enough.
        {
            date = date.AddDays(-1);
            if (date.DayOfWeek == dayOfWeek) return date;
        }
        throw new Exception("Previous day of week was not found. We should never hit this exception or else this logic should be reviewed.");
    }

    /// <summary>
    /// Returns the start of the month for the given date, optionally adding months and ensuring it is a weekday.
    /// </summary>
    /// <param name="date"></param>
    /// <param name="months_to_add"></param>
    /// <param name="weekday"></param>
    /// <returns></returns>
    public static DateTime StartOfMonth(this DateTime date, int months_to_add = 0, bool weekday = false)
    {
        date = date.AddMonths(months_to_add);
        DateTime startOfMonth = new(date.Year, date.Month, 1);
        return weekday ? startOfMonth.WeekdayOrNext() : startOfMonth;
    }

    /// <summary>
    /// Returns the end of the month for the given date, optionally adding months and ensuring it is a weekday.
    /// </summary>
    /// <param name="date"></param>
    /// <param name="monthsToAdd"></param>
    /// <param name="weekday"></param>
    /// <returns></returns>
    public static DateTime EndOfMonth(this DateTime date, int monthsToAdd = 0, bool weekday = false)
    {
        date = date.AddMonths(monthsToAdd);

        DateTime endOfMonth = new(date.Year,
                               date.Month,
                               DateTime.DaysInMonth(date.Year, date.Month));
        return weekday ? endOfMonth.WeekdayOrPrevious() : endOfMonth;
    }

    /// <summary>
    /// Checks if the date is a weekday (Monday to Friday).
    /// </summary>
    /// <param name="date"></param>
    /// <returns></returns>
    public static bool IsWeekday(this DateTime date) => !date.IsWeekend();

    /// <summary>
    /// Checks if the date is a weekend (Saturday or Sunday).
    /// </summary>
    /// <param name="date"></param>
    /// <returns></returns>
    public static bool IsWeekend(this DateTime date) => (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday);

    /// <summary>
    /// Returns the number of weekdays between two dates, excluding the start date.
    /// </summary>
    /// <param name="start_date"></param>
    /// <param name="end_date"></param>
    /// <returns></returns>
    public static int WeekdaysFromDate(this DateTime start_date, DateTime end_date) => DateOnlyExtension.WeekdaysFromDate(start_date.ToDateOnly(), end_date.ToDateOnly());


    /// <summary>
    /// Returns 1 for the first half and 2 for the second half.
    /// </summary>
    /// <param name="Date"></param>
    /// <returns></returns>
    public static int HalfYear(this DateTime Date) => Date.Month <= 6 ? 1 : 2;

    /// <summary>
    /// Returns the quarter of the year.
    /// </summary>
    /// <param name="Date"></param>
    /// <returns></returns>
    public static int Quarter(this DateTime Date)
    {
        return Convert.ToInt32(Math.Ceiling((decimal)Date.Month / 3));
    }

    /// <summary>
    /// Returns the start of the year for the given date, optionally ensuring it is a weekday.
    /// </summary>
    /// <param name="date"></param>
    /// <param name="weekday"></param>
    /// <returns></returns>
    public static DateTime StartOfYear(this DateTime date, bool weekday = false)
    {
        DateTime startOfYear = new(date.Year, 1, 1);
        return weekday ? startOfYear.WeekdayOrNext() : startOfYear;
    }

    /// <summary>
    /// Returns the end of the year for the given date, optionally ensuring it is a weekday.
    /// </summary>
    /// <param name="date"></param>
    /// <param name="weekday"></param>
    /// <returns></returns>
    public static DateTime EndOfYear(this DateTime date, bool weekday = false)
    {
        DateTime endOfYear = new(date.Year, 12, 31);
        return weekday ? endOfYear.WeekdayOrPrevious() : endOfYear;
    }

    /// <summary>
    /// Returns the start of the quarter for the given date, optionally ensuring it is a weekday.
    /// </summary>
    /// <param name="date"></param>
    /// <param name="weekday"></param>
    /// <returns></returns>
    public static DateTime StartOfQuarter(this DateTime date, bool weekday = false)
    {
        int month = (date.Quarter() - 1) * 3 + 1;
        DateTime startOfQuarter = new(date.Year, month, 1);
        return weekday ? startOfQuarter.WeekdayOrNext() : startOfQuarter;
    }

    /// <summary>
    /// Returns the end of the quarter for the given date, optionally ensuring it is a weekday.
    /// </summary>
    /// <param name="date"></param>
    /// <param name="weekday"></param>
    /// <returns></returns>
    public static DateTime EndOfQuarter(this DateTime date, bool weekday = false)
    {
        int month = (date.Quarter() * 3);
        DateTime endOfQuarter = new(date.Year, month, DateTime.DaysInMonth(date.Year, month));
        return weekday ? endOfQuarter.WeekdayOrPrevious() : endOfQuarter;
    }

    /// <summary>
    /// Returns the start of the half-year for the given date, optionally ensuring it is a weekday.
    /// </summary>
    /// <param name="date"></param>
    /// <param name="weekday"></param>
    /// <returns></returns>
    public static DateTime StartOfHalfYear(this DateTime date, bool weekday = false)
    {
        int month = (date.HalfYear() - 1) * 6 + 1;
        DateTime startOfHalfYear = new(date.Year, month, 1);
        return weekday ? startOfHalfYear.WeekdayOrNext() : startOfHalfYear;
    }

    /// <summary>
    /// Returns the end of the half-year for the given date, optionally ensuring it is a weekday.
    /// </summary>
    /// <param name="date"></param>
    /// <param name="weekday"></param>
    /// <returns></returns>
    public static DateTime EndOfHalfYear(this DateTime date, bool weekday = false)
    {
        int month = date.HalfYear() * 6;
        DateTime endOfHalfYear = new(date.Year, month, DateTime.DaysInMonth(date.Year, month));
        return weekday ? endOfHalfYear.WeekdayOrPrevious() : endOfHalfYear;
    }

    /// <summary>
    /// Counts the occurrences of a specific day of the week between two dates, inclusive.
    /// </summary>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <param name="day_of_week"></param>
    /// <returns></returns>
    public static int CountDaysOfWeekBetweenDates(this DateTime start, DateTime end, DayOfWeek day_of_week)
    {
        return DateOnlyExtension.CountDaysOfWeekBetweenDates(start.ToDateOnly(), end.ToDateOnly(), day_of_week);
    }
}
