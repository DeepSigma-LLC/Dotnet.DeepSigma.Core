using DeepSigma.Core.Enums;

namespace DeepSigma.Core;

/// <summary>
/// Represents a periodicity configuration including periodicity type, day selection, and time interval.
/// </summary>
public readonly struct PeriodicityConfiguration
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PeriodicityConfiguration"/> struct.
    /// </summary>
    /// <param name="periodicity"></param>
    /// <param name="daySelection"></param>
    /// <param name="timeInterval"></param>
    public PeriodicityConfiguration(Periodicity periodicity, DaySelectionType daySelection = DaySelectionType.Any, TimeInterval? timeInterval = null)
    {
        this.Periodicity = periodicity;
        this.DayType = daySelection;
        this.Time = timeInterval;
    }

    /// <summary>
    /// The periodicity type.
    /// </summary>
    public Periodicity Periodicity { get; init; }

    /// <summary>
    /// The day selection type.
    /// </summary>
    public DaySelectionType DayType { get; init; } = DaySelectionType.Any;

    /// <summary>
    /// Gets or sets the required day of the week for weekly periodicity. If the periodicity is not weekly, this property will be null.
    /// </summary>
    public DayOfWeek? RequiredDayOfWeek {get; init;} = null;

    /// <summary>
    /// Gets or sets the time interval in minutes for daily periodicity. If the periodicity is not daily, this property will be null.
    /// </summary>
    public TimeInterval? Time
    { 
        get => Periodicity.Daily == this.Periodicity ? field : null;
        init => field = value;
    } = null;
}
