
namespace DeepSigma.Core.Enums;

/// <summary>
/// Represents various time intervals in minutes.
/// </summary>
public enum TimeInterval
{
    /// <summary>
    /// Represents a time interval of 1 minute.
    /// </summary>
    Min_1 = 1,
    /// <summary>
    /// Represents a time interval of 2 minutes.
    /// </summary>
    Min_2 = 2,
    /// <summary>
    /// Represents a time interval of 3 minutes.
    /// </summary>
    Min_3 = 3,
    /// <summary>
    /// Represents a time interval of 5 minutes.
    /// </summary>
    Min_5 = 5,
    /// <summary>
    /// Represents a time interval of 10 minutes.
    /// </summary>
    Min_10 = 10,
    /// <summary>
    /// Represents a time interval of 15 minutes.
    /// </summary>
    Min_15 = 15,
    /// <summary>
    /// Represents a time interval of 20 minutes.
    /// </summary>
    Min_20 = 20,
    /// <summary>
    /// Represents a time interval of 30 minutes.
    /// </summary>
    Min_30 = 30,
    /// <summary>
    /// Represents a time interval of 45 minutes.
    /// </summary>
    Min_45 = 45,
    /// <summary>
    /// Represents a time interval of 60 minutes.
    /// </summary>
    Min_60 = 60,
    /// <summary>
    /// Represents a time interval of 90 minutes.
    /// </summary>
    Min_90 = 90,
    /// <summary>
    /// Represents a time interval of 120 minutes.
    /// </summary>
    Min_120 = 120,
    /// <summary>
    /// Represents a time interval of 180 minutes.
    /// </summary>
    Min_180 = 180
}

/// <summary>
/// Provides extension methods for the TimeInterval enum.
/// </summary>
public static class TimeIntervalExtensions
{
    /// <summary>
    /// Converts the TimeInterval enum value to its equivalent TimeSpan representation.
    /// </summary>
    /// <param name="timeInterval">The TimeInterval enum value.</param>
    /// <returns>A TimeSpan representing the time interval.</returns>
    public static TimeSpan ToTimeSpan(this TimeInterval timeInterval)
    {
        int minutes = timeInterval switch
        {
            TimeInterval.Min_1 => 1,
            TimeInterval.Min_2 => 2,
            TimeInterval.Min_3 => 3,
            TimeInterval.Min_5 => 5,
            TimeInterval.Min_10 => 10,
            TimeInterval.Min_15 => 15,
            TimeInterval.Min_20 => 20,
            TimeInterval.Min_30 => 30,
            TimeInterval.Min_45 => 45,
            TimeInterval.Min_60 => 60,
            TimeInterval.Min_90 => 90,
            TimeInterval.Min_120 => 120,
            TimeInterval.Min_180 => 180,
            _ => throw new ArgumentOutOfRangeException(nameof(timeInterval), timeInterval, null),
        };
        return TimeSpan.FromMinutes((int)timeInterval);
    }
}