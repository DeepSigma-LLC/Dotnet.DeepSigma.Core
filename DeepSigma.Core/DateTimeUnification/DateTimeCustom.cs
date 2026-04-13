using DeepSigma.Core.Extensions;
using System;
using System.ComponentModel;

namespace DeepSigma.Core.DateTimeUnification;

/// <summary>
/// Represents a custom date object that encapsulates a DateTime value. 
/// </summary>
/// <param name="date_time"></param>
[TypeConverter(typeof(CustomDateTypeConverter<DateTimeCustom>))]
public readonly struct DateTimeCustom(DateTime date_time) : IDateTime<DateTimeCustom>
{
    private DateTime _dateTime { get; } = date_time;

    /// <inheritdoc cref="DateTimeCustom"/>
    public DateTimeCustom(int year, int month, int day) : this(new DateTime(year, month, day)) { }

    /// <inheritdoc cref="DateTimeCustom"/>
    public DateTimeCustom(int year, int month, int day, int hour, int minute, int second) : this(new DateTime(year, month, day, hour, minute, second)) { }

    /// <inheritdoc cref="DateTimeCustom"/>
    public DateTimeCustom(DateOnly date) : this(date.ToDateTime()) { }

    /// <inheritdoc/>
    public static DateTimeCustom Create(DateTime date_time) => new(date_time);

    /// <inheritdoc/>
    public static implicit operator DateTimeCustom(DateTime dt) => new(dt);

    /// <inheritdoc/>
    public static implicit operator DateTimeCustom(DateOnly d) => new(d);

    /// <inheritdoc/>
    public static implicit operator DateOnly(DateTimeCustom dc) => dc.DateOnly;

    /// <inheritdoc/>
    public static implicit operator DateTime(DateTimeCustom dc) => dc.DateTime;

    /// <inheritdoc/>
    public static implicit operator DateTimeOffset(DateTimeCustom dc) => dc.DateTimeOffset;

    /// <inheritdoc/>
    public static implicit operator DateTimeCustom(DateTimeOffset dto) => new(dto.DateTime);

    /// <inheritdoc/>
    public DateOnly DateOnly => _dateTime.ToDateOnly();

    /// <inheritdoc/>
    public DateTime DateTime => _dateTime;

    /// <inheritdoc/>
    public DateTimeOffset DateTimeOffset => _dateTime;

    /// <inheritdoc/>
    public DateOnly Date => _dateTime.Date.ToDateOnly();

    /// <inheritdoc/>
    public int Day => _dateTime.Day;

    /// <inheritdoc/>
    public int Month => _dateTime.Month;

    /// <inheritdoc/>
    public int Year => _dateTime.Year;

    /// <inheritdoc/>
    public static DateTimeCustom Now => DateTime.Now;

    /// <inheritdoc/>
    public static DateTimeCustom Today => DateTime.Now.Date;

    /// <inheritdoc/>
    public static DateTimeCustom NowUtc => DateTime.UtcNow;

    /// <inheritdoc/>
    public DayOfWeek DayOfWeek => _dateTime.DayOfWeek;

    /// <inheritdoc/>
    public int DayOfYear => _dateTime.DayOfYear;

    /// <inheritdoc/>
    public static DateTimeCustom MinValue => DateTime.MinValue;

    /// <inheritdoc/>
    public static DateTimeCustom MaxValue => DateTime.MaxValue;

    /// <inheritdoc/>
    public bool IsLeapYear => DateTime.IsLeapYear(Year);

    /// <inheritdoc/>
    public int DaysInMonth => DateTime.DaysInMonth(Year, Month);

    /// <inheritdoc/>
    public int DaysInYear => DateTimeExtension.GetDaysInYear(DateTime);

    /// <inheritdoc/>
    public int Quarter => DateTimeExtension.Quarter(DateTime);

    /// <inheritdoc/>
    public int HalfYear => DateTimeExtension.HalfYear(DateTime);

    /// <inheritdoc/>
    public DateTimeCustom AddDays(int value) => _dateTime.AddDays(value);

    /// <inheritdoc/>
    public DateTimeCustom AddMonths(int months) => _dateTime.AddMonths(months);
    
    /// <inheritdoc/>
    public DateTimeCustom AddYears(int value) => _dateTime.AddYears(value);
    
    /// <inheritdoc/>
    public void Deconstruct(out int year, out int month, out int day)
    {
        year = _dateTime.Year;
        month = _dateTime.Month;
        day = _dateTime.Day;
    }

    /// <inheritdoc/>
    public static TimeSpan operator -(DateTimeCustom dt, DateTimeCustom cd) => dt.DateTime - cd.DateTime;

    /// <inheritdoc/>
    public static bool operator >(DateTimeCustom cd1, DateTimeCustom cd2) => cd1._dateTime > cd2._dateTime;

    /// <inheritdoc/>
    public static bool operator <(DateTimeCustom cd1, DateTimeCustom cd2) => cd1._dateTime < cd2._dateTime;

    /// <inheritdoc/>
    public static bool operator >=(DateTimeCustom cd1, DateTimeCustom cd2) => cd1._dateTime >= cd2._dateTime;

    /// <inheritdoc/>
    public static bool operator <=(DateTimeCustom cd1, DateTimeCustom cd2) => cd1._dateTime <= cd2._dateTime;

    /// <inheritdoc/>
    public static bool operator ==(DateTimeCustom cd1, DateTimeCustom cd2) => cd1._dateTime == cd2._dateTime;

    /// <inheritdoc/>
    public static bool operator !=(DateTimeCustom cd1, DateTimeCustom cd2) => cd1._dateTime != cd2._dateTime;

    /// <inheritdoc/>
    public override int GetHashCode() => _dateTime.GetHashCode();

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
        if (obj is not DateTimeCustom other) return false;
        return _dateTime.Equals(other._dateTime);
    }

    /// <summary>
    /// Returns the string representation of the date in "yyyy-MM-dd" format.
    /// </summary>
    /// <returns></returns>
    public override string ToString() => _dateTime.ToString("yyyy-MM-dd");

    /// <inheritdoc/>
    public DateTimeCustom EndOfMonth(int months_to_add = 0, bool must_be_weekday = false)
    {
        return DateTimeExtension.EndOfMonth(_dateTime, months_to_add, must_be_weekday);
    }

    /// <inheritdoc/>
    public DateTimeCustom StartOfMonth(int months_to_add = 0, bool must_be_weekday = false)
    {
        return DateTimeExtension.StartOfMonth(_dateTime, months_to_add, must_be_weekday);
    }

    /// <inheritdoc/>
    public DateTimeCustom AddWeekdays(int weekdays) => DateTimeExtension.AddWeekdays(_dateTime, weekdays);

    /// <inheritdoc/>
    public bool IsWeekday() => DateTimeExtension.IsWeekday(_dateTime);

    /// <inheritdoc/>
    public bool IsWeekend() => DateTimeExtension.IsWeekend(_dateTime);
    
    /// <inheritdoc/>
    public DateTimeCustom NextDayOfWeekSpecified(DayOfWeek day_of_week)
    {
        return DateTimeExtension.NextDayOfWeekSpecified(_dateTime, day_of_week);
    }

    /// <inheritdoc />
    public DateTimeCustom PreviousDayOfWeekSpecified(DayOfWeek day_of_week)
    {
        return DateTimeExtension.NextDayOfWeekSpecified(DateTime, day_of_week);
    }

    /// <inheritdoc/>
    public int CompareTo(DateTimeCustom other) => _dateTime.CompareTo(other._dateTime);

    /// <inheritdoc/>
    public int CompareTo(object? obj) => _dateTime.CompareTo(obj);


    /// <inheritdoc cref="DateTime.Parse(string)"/>
    public static DateTimeCustom Parse(string s) => new(DateTime.Parse(s));

    /// <inheritdoc/>
    public DateTimeCustom AddWeekendDays(int weekend_days)
    {
        return DateTimeExtension.AddWeekendDays(_dateTime, weekend_days);
    }
}

