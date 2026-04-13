using DeepSigma.Core.Extensions; 
using System.ComponentModel;

namespace DeepSigma.Core.DateTimeUnification;

/// <summary>
/// Represents a custom date object that encapsulates a DateTimeOffset value.
/// </summary>
[TypeConverter(typeof(CustomDateTypeConverter<DateTimeOffsetCustom>))]
public readonly struct DateTimeOffsetCustom : IDateTime<DateTimeOffsetCustom>
{
    private readonly DateTimeOffset _dateTimeOffset;

    /// <inheritdoc cref="DateTimeOffsetCustom"/>
    DateTimeOffsetCustom(DateTimeOffset dateTimeOffset)
    {
        _dateTimeOffset = dateTimeOffset;
    }

    /// <inheritdoc cref="DateTimeOffsetCustom"/>
    public DateTimeOffsetCustom(DateTime dateTime)
    {
        _dateTimeOffset = new DateTimeOffset(dateTime);
    }

    /// <inheritdoc cref="DateTimeOffsetCustom"/>
    public DateTimeOffsetCustom(DateOnly dateOnly)
    {
        _dateTimeOffset = new DateTimeOffset(dateOnly.ToDateTime(TimeOnly.MinValue));
    }

    /// <inheritdoc/>
    public static implicit operator DateTimeOffsetCustom(DateTime dt) => new(dt);

    /// <inheritdoc/>
    public static implicit operator DateTimeOffsetCustom(DateOnly d) => new(d);

    /// <inheritdoc/>
    public static implicit operator DateOnly(DateTimeOffsetCustom dc) => dc.DateOnly;

    /// <inheritdoc/>
    public static implicit operator DateTime(DateTimeOffsetCustom dc) => dc.DateTime;

    /// <inheritdoc/>
    public static implicit operator DateTimeOffset(DateTimeOffsetCustom dc) => dc.DateTimeOffset;

    /// <inheritdoc/>
    public static implicit operator DateTimeOffsetCustom(DateTimeOffset dto) => new(dto);

    /// <inheritdoc/>
    public static DateTimeOffsetCustom Create(DateTime date_time) => new(date_time);

    /// <inheritdoc/>
    public static DateTimeOffsetCustom MaxValue => DateTimeOffset.MaxValue;

    /// <inheritdoc/>
    public static DateTimeOffsetCustom MinValue => DateTimeOffset.MinValue;

    /// <inheritdoc/>
    public static DateTimeOffsetCustom Now => DateTimeOffset.Now;

    /// <inheritdoc/>
    public static DateTimeOffsetCustom NowUtc => DateTimeOffset.UtcNow;

    /// <inheritdoc/>
    public static DateTimeOffsetCustom Today => DateTimeOffset.Now.Date;

    /// <inheritdoc/>
    public DateOnly Date => _dateTimeOffset.DateTime.ToDateOnly();

    /// <inheritdoc/>
    public DateOnly DateOnly => _dateTimeOffset.DateTime.ToDateOnly();

    /// <inheritdoc/>
    public DateTime DateTime => _dateTimeOffset.DateTime;

    /// <inheritdoc/>
    public DateTimeOffset DateTimeOffset => _dateTimeOffset;

    /// <inheritdoc/>
    public int Day => _dateTimeOffset.Day;

    /// <inheritdoc/>
    public DayOfWeek DayOfWeek => _dateTimeOffset.DayOfWeek;

    /// <inheritdoc/>
    public int DayOfYear => _dateTimeOffset.DayOfYear;

    /// <inheritdoc/>
    public int DaysInMonth => DateTime.DaysInMonth(Year, Month);

    /// <inheritdoc/>
    public bool IsLeapYear => DateTime.IsLeapYear(Year);

    /// <inheritdoc/>
    public int Month => _dateTimeOffset.Month;

    /// <inheritdoc/>
    public int Year => _dateTimeOffset.Year;

    /// <inheritdoc/>
    public int DaysInYear => DateTimeExtension.GetDaysInYear(DateTime);

    /// <inheritdoc/>
    public int Quarter => DateTimeExtension.Quarter(DateTime);

    /// <inheritdoc/>
    public int HalfYear => DateTimeExtension.HalfYear(DateTime);

    /// <inheritdoc/>
    public DateTimeOffsetCustom AddDays(int value) => _dateTimeOffset.AddDays(value);

    /// <inheritdoc/>
    public DateTimeOffsetCustom AddMonths(int months) => _dateTimeOffset.AddMonths(months);

    /// <inheritdoc/>
    public DateTimeOffsetCustom AddWeekdays(int weekdays) => _dateTimeOffset.DateTime.AddWeekdays(weekdays);

    /// <inheritdoc/>
    public DateTimeOffsetCustom AddYears(int value) => new(_dateTimeOffset.AddYears(value));

    /// <inheritdoc/>
    public void Deconstruct(out int year, out int month, out int day)
    {
        year = _dateTimeOffset.Year;
        month = _dateTimeOffset.Month;
        day = _dateTimeOffset.Day;
    }

    /// <inheritdoc/>
    public DateTimeOffsetCustom EndOfMonth(int months_to_add = 0, bool must_be_weekday = false)
    {
        return new(_dateTimeOffset.DateTime.EndOfMonth(months_to_add, must_be_weekday));
    }

    /// <inheritdoc/>
    public bool IsWeekday() => DateTimeExtension.IsWeekday(DateTime);

    /// <inheritdoc/>
    public bool IsWeekend() => DateTimeExtension.IsWeekend(DateTime);

    /// <inheritdoc/>
    public DateTimeOffsetCustom NextDayOfWeekSpecified(DayOfWeek day_of_week)
    {
        return _dateTimeOffset.DateTime.NextDayOfWeekSpecified(day_of_week);
    }

    /// <inheritdoc />
    public DateTimeOffsetCustom PreviousDayOfWeekSpecified(DayOfWeek day_of_week)
    {
        return _dateTimeOffset.DateTime.PreviousDayOfWeekSpecified(day_of_week);
    }

    /// <inheritdoc/>
    public DateTimeOffsetCustom StartOfMonth(int months_to_add = 0, bool must_be_weekday = false)
    {
        return _dateTimeOffset.DateTime.StartOfMonth(months_to_add, must_be_weekday);
    }

    /// <inheritdoc/>
    public static TimeSpan operator -(DateTimeOffsetCustom left, DateTimeOffsetCustom right) => left.DateTimeOffset - right.DateTimeOffset;

    /// <inheritdoc/>
    public static bool operator ==(DateTimeOffsetCustom left, DateTimeOffsetCustom right) => left.Equals(right);

    /// <inheritdoc/>
    public static bool operator !=(DateTimeOffsetCustom left, DateTimeOffsetCustom right) => !(left == right);

    /// <inheritdoc/>
    public static bool operator <(DateTimeOffsetCustom left, DateTimeOffsetCustom right) => left < right;

    /// <inheritdoc/>
    public static bool operator >(DateTimeOffsetCustom left, DateTimeOffsetCustom right) => left > right;

    /// <inheritdoc/>
    public static bool operator <=(DateTimeOffsetCustom left, DateTimeOffsetCustom right) => left <= right;

    /// <inheritdoc/>
    public static bool operator >=(DateTimeOffsetCustom left, DateTimeOffsetCustom right) => left >= right;

    /// <inheritdoc/>
    public override int GetHashCode() => _dateTimeOffset.GetHashCode();

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
        if (obj is not DateTimeOffsetCustom dto) return false;
        return _dateTimeOffset.Equals(dto._dateTimeOffset);
    }

    /// <inheritdoc/>
    public int CompareTo(DateTimeOffsetCustom other) => _dateTimeOffset.CompareTo(other._dateTimeOffset);

    /// <inheritdoc/>
    public int CompareTo(object? obj) => (_dateTimeOffset as IComparable).CompareTo(obj);

    /// <inheritdoc cref="DateTimeOffset.Parse(string)"/>
    public static DateTimeOffsetCustom Parse(string s) => new(DateTimeOffset.Parse(s));

    /// <inheritdoc/>
    public DateTimeOffsetCustom AddWeekendDays(int weekend_days)
    {
        return _dateTimeOffset.DateTime.AddWeekendDays(weekend_days);
    }

}
