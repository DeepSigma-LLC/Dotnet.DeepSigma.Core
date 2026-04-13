
namespace DeepSigma.Core.Enums;

/// <summary>
/// Represents the type of day selection for scheduling or filtering purposes.
/// </summary>
public enum DaySelectionType
{
    /// <summary>
    /// Day selection is not specified.
    /// </summary>
    Any,
    /// <summary>
    /// Day selection must be a weekday.
    /// </summary>
    Weekday,
    /// <summary>
    /// Day selection must be a weekend day.
    /// </summary>
    Weekend,
    /// <summary>
    /// Day selection must be a specific day of the week.
    /// </summary>
    SpecificDayOfWeek
}

