using DeepSigma.Core.Extensions;
using System.ComponentModel;

namespace DeepSigma.Core.DateTimeUnification;

/// <summary>
/// Represents a custom date object that encapsulates a DateOnly value.
/// </summary>
/// <param name="date_only"></param>
[TypeConverter(typeof(CustomDateTypeConverter<DateOnlyCustom>))]
public readonly struct DateOnlyCustom(DateOnly date_only) : IDateTime<DateOnlyCustom>
{
    private readonly DateOnly _date_only = date_only;

    /// <inheritdoc cref="DateOnlyCustom"/>
    public DateOnlyCustom(int year, int month, int day) : this(new DateOnly(year, month, day)) {}

    /// <inheritdoc cref="DateOnlyCustom"/>
    public DateOnlyCustom(DateTime dateTime) : this(dateTime.ToDateOnly()) { }

    /// <inheritdoc/>
    public static DateOnlyCustom Create(DateTime date_time) => new(date_time);

    /// <inheritdoc/>
    public static implicit operator DateOnlyCustom(DateTime dt) => new(dt);

    /// <inheritdoc/>
    public static implicit operator DateOnlyCustom(DateOnly d) => new(d);

    /// <inheritdoc/>
    public static implicit operator DateOnly(DateOnlyCustom dc) => dc._date_only;

    /// <inheritdoc/>
    public static implicit operator DateTime(DateOnlyCustom dc) => dc.DateTime;

    /// <inheritdoc/>
    public static implicit operator DateTimeOffset(DateOnlyCustom dc) => dc.DateTimeOffset;

    /// <inheritdoc/>
    public static implicit operator DateOnlyCustom(DateTimeOffset dto) => new(dto.DateTime);

    /// <inheritdoc/>
    public DateTime DateTime => _date_only.ToDateTime();

    /// <inheritdoc/>
    public DateTimeOffset DateTimeOffset => DateTime;

    /// <inheritdoc/>
    public DateOnly DateOnly  => _date_only;

    /// <inheritdoc/>
    public int Year => _date_only.Year;

    /// <inheritdoc/>
    public int Month => _date_only.Month;

    /// <inheritdoc/>
    public int Day => _date_only.Day;

    /// <inheritdoc/>
    public static DateOnlyCustom MaxValue => DateOnly.MaxValue;

    /// <inheritdoc/>
    public static DateOnlyCustom MinValue => DateOnly.MinValue;

    /// <inheritdoc/>
    public static DateOnlyCustom Now => DateTime.Today;

    /// <inheritdoc/>
    public static DateOnlyCustom NowUtc => DateTime.UtcNow;

    /// <inheritdoc/>
    public static DateOnlyCustom Today => DateTime.Today;

    /// <inheritdoc/>
    public DateOnly Date => _date_only;

    /// <inheritdoc/>
    public DayOfWeek DayOfWeek => _date_only.DayOfWeek;

    /// <inheritdoc/>
    public int DayOfYear => _date_only.DayOfYear;

    /// <inheritdoc/>
    public int DaysInMonth => DateTime.DaysInMonth(Year, Month);

    /// <inheritdoc/>
    public bool IsLeapYear => DateTime.IsLeapYear(Year);

    /// <inheritdoc/>
    public int DaysInYear => DateTimeExtension.GetDaysInYear(DateTime);

    /// <inheritdoc/>
    public int Quarter => DateTimeExtension.Quarter(DateTime);

    /// <inheritdoc/>
    public int HalfYear => DateTimeExtension.HalfYear(DateTime);

    /// <inheritdoc/>
    public DateOnlyCustom AddDays(int value) => _date_only.AddDays(value);

    /// <inheritdoc/>
    public DateOnlyCustom AddMonths(int months) => _date_only.AddMonths(months);

    /// <inheritdoc/>
    public DateOnlyCustom AddYears(int value) => _date_only.AddYears(value);

    /// <inheritdoc/>
    public void Deconstruct(out int year, out int month, out int day)
    {
        year = _date_only.Year;
        month = _date_only.Month;
        day = _date_only.Day;
    }

    /// <inheritdoc/>
    public static TimeSpan operator -(DateOnlyCustom dt, DateOnlyCustom cd) => dt.DateTime - cd.DateTime;

    /// <inheritdoc/>
    public static bool operator >(DateOnlyCustom cd1, DateOnlyCustom cd2) => cd1._date_only > cd2._date_only;

    /// <inheritdoc/>
    public static bool operator <(DateOnlyCustom cd1, DateOnlyCustom cd2) => cd1._date_only < cd2._date_only;

    /// <inheritdoc/>
    public static bool operator >=(DateOnlyCustom cd1, DateOnlyCustom cd2) => cd1._date_only >= cd2._date_only;

    /// <inheritdoc/>
    public static bool operator <=(DateOnlyCustom cd1, DateOnlyCustom cd2) => cd1._date_only <= cd2._date_only;

    /// <inheritdoc/>
    public static bool operator ==(DateOnlyCustom cd1, DateOnlyCustom cd2) => cd1._date_only == cd2._date_only;

    /// <inheritdoc/>
    public static bool operator !=(DateOnlyCustom cd1, DateOnlyCustom cd2) => cd1._date_only != cd2._date_only;

    /// <summary>
    /// Returns the string representation of the date in "yyyy-MM-dd" format.
    /// </summary>
    /// <returns></returns>
    public override string ToString() => _date_only.ToString("yyyy-MM-dd");

    /// <inheritdoc/>
    public override int GetHashCode() => _date_only.GetHashCode();

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
        if (obj is not DateOnlyCustom other) return false;
        return _date_only.Equals(other._date_only);
    }

    /// <inheritdoc/>
    public DateOnlyCustom EndOfMonth(int months_to_add = 0, bool must_be_weekday = false)
    {
        return DateTimeExtension.EndOfMonth(DateTime, months_to_add, must_be_weekday);
    }

    /// <inheritdoc/>
    public DateOnlyCustom StartOfMonth(int months_to_add = 0, bool must_be_weekday = false)
    {
        return DateTimeExtension.StartOfMonth(DateTime, months_to_add, must_be_weekday);
    }

    /// <inheritdoc/>
    public DateOnlyCustom AddWeekdays(int weekdays) => DateTimeExtension.AddWeekdays(DateTime, weekdays);

    /// <inheritdoc/>
    public bool IsWeekday() => DateTimeExtension.IsWeekday(DateTime);
    
    /// <inheritdoc/>
    public bool IsWeekend() => DateTimeExtension.IsWeekend(DateTime);

    /// <inheritdoc/>
    public DateOnlyCustom NextDayOfWeekSpecified(DayOfWeek day_of_week)
    {
        return DateTimeExtension.NextDayOfWeekSpecified(DateTime, day_of_week);
    }

    /// <inheritdoc />
    public DateOnlyCustom PreviousDayOfWeekSpecified(DayOfWeek day_of_week)
    {
        return DateTimeExtension.NextDayOfWeekSpecified(DateTime, day_of_week);
    }

    /// <inheritdoc/>
    public int CompareTo(DateOnlyCustom other) => _date_only.CompareTo(other._date_only);

    /// <inheritdoc/>
    public int CompareTo(object? obj) => _date_only.CompareTo(obj);


    /// <inheritdoc cref="DateOnly.Parse(string)"/>
    public static DateOnlyCustom Parse(string s) => new(DateOnly.Parse(s));

    /// <inheritdoc/>
    public DateOnlyCustom AddWeekendDays(int weekend_days)
    {
        return DateOnlyExtension.AddWeekendDays(DateOnly, weekend_days);
    }
}
