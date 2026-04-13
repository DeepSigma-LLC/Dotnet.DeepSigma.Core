using DeepSigma.Core.DateTimeUnification;
using DeepSigma.Core.Enums;
using DeepSigma.Core.Extensions;

namespace DeepSigma.Core.TimeStepper;

/// <summary>
/// Self-aligning time stepper <see cref="SelfAligningTimeStepper{T}"/> is a used to calculate time steps based on periodicity configuration.
/// The goal of this class is to provide time steps that align with specific periodicity rules, such as month-end or quarter-end dates.
/// This class supports various periodicities including intraday, daily, weekly, monthly, quarterly, semi-annual, and annual.
/// </summary>
/// <remarks>
/// The type parameter T must implement <see cref="IDateTime{T}"/> to ensure compatibility with date-time operations.
/// The goal is to provide a flexible and reusable component for time step calculations in scheduling or time series applications.
/// <para>The method enforces the following rules:</para>
/// <list type="number">
///   <item>
///     <description>
///     <b>DateOnlyCustom cannot be used with a time component.</b><br/>
///     If the generic type <c>T</c> is <see cref="DateOnlyCustom"/> and the configuration 
///     includes a time value, the setup is invalid because <see cref="DateOnlyCustom"/> 
///     does not support time-based intervals.
///     </description>
///   </item>
///
///   <item>
///     <description>
///     <b>A required day of week must be provided for weekly schedules.</b><br/>
///     When the periodicity is weekly, a specific <see cref="DayOfWeek"/> value must be supplied.
///     </description>
///   </item>
///
///   <item>
///     <description>
///     <b>A required day of week must be provided when using 
///     <see cref="DaySelectionType.SpecificDayOfWeek"/>.</b><br/>
///     If alignment depends on a specific day of the week, that day must be specified.
///     </description>
///   </item>
///
///   <item>
///     <description>
///     <b>A required day of week must not be provided unless it is needed.</b><br/>
///     Only weekly periodicity or <see cref="DaySelectionType.SpecificDayOfWeek"/> 
///     should include a day-of-week value.  
///     For all other configurations, the day-of-week parameter must be <c>null</c>.
///     </description>
///   </item>
/// </list>
/// </remarks>
public class SelfAligningTimeStepper<T>
    where T : struct, IDateTime<T>, IComparable<T>
{
    /// <summary>
    /// Configuration for time stepping operations.
    /// </summary>
    private TimeStepperConfiguration Configuration {  get; set; }

    /// <summary>
    /// Gets move direction scalar based on AdjustmentType and MoveForward flag.
    /// </summary>
    /// <param name="MoveForward"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    private int GetAdjustmentDirectionScalar(bool MoveForward)
    {
        return Configuration.AdjustmentType switch
        {
            DateAdjustmentType.MoveForward => 1,
            DateAdjustmentType.MoveBackward => -1,
            DateAdjustmentType.MoveInDirectionOfTimeStep => MoveForward ? 1 : -1,
            _ => throw new NotImplementedException(),
        };
    }

    /// <summary>
    /// Time span stepper for intraday time step calculations.
    /// </summary>
    private TimeSpanStepper<T>? TimeSpanStepper { get; init; }

    /// <inheritdoc cref="SelfAligningTimeStepper{TimeStepperConfiguration}"/>
    public SelfAligningTimeStepper(TimeStepperConfiguration configuration)
    {
        ValidateDaySelection(configuration);
        this.Configuration = configuration;

        if(configuration.PeriodicityConfig.Time is not null)
        {
            this.TimeSpanStepper = new(configuration.PeriodicityConfig.Time.Value);
        }
    }

    /// <summary>
    /// Validates the day selection configuration.
    /// </summary>
    /// <param name="configuration"></param>
    /// <exception cref="ArgumentException"></exception>
    private void ValidateDaySelection(TimeStepperConfiguration configuration)
    {
        if (typeof(T) == typeof(DateOnlyCustom) && configuration.PeriodicityConfig.Time is not null)
            throw new ArgumentException($"Time interval is not supported for {nameof(DateOnlyCustom)} type.");

        if (configuration.PeriodicityConfig.Periodicity == Periodicity.Daily && configuration.AdjustmentType != DateAdjustmentType.MoveInDirectionOfTimeStep)
            throw new ArgumentException("Adjustment type must be MoveInDirectionOfTimeStep for daily periodicities to prevent looping. " +
                "We always move in the direction of the time step for daily periodicities.");

        if (configuration.PeriodicityConfig.Periodicity == Periodicity.Weekly && configuration.RequiredDayOfWeek is null) 
            throw new ArgumentException("Required day of week must be provided for weekly periodicity.");

        if (configuration.PeriodicityConfig.DayType == DaySelectionType.SpecificDayOfWeek && configuration.RequiredDayOfWeek is null)
            throw new ArgumentException("Required day of week must be provided when day selection is specific day of week.");

        if (
            !(configuration.PeriodicityConfig.DayType == DaySelectionType.SpecificDayOfWeek || configuration.PeriodicityConfig.Periodicity == Periodicity.Weekly)
            && configuration.RequiredDayOfWeek is not null
            )
            throw new ArgumentException("Required day of week should be null unless day selection is specific day of week or periodicity is weekly.");
    }

    /// <summary>
    /// Generates a set of date-times between StartDate and EndDate based on the periodicity configuration.
    /// </summary>
    /// <param name="StartDate"></param>
    /// <param name="EndDate"></param>
    /// <param name="IncludeStartAndEndDates"></param>
    /// <returns></returns>
    public HashSet<T> GetDateTimes(T StartDate, T EndDate, bool IncludeStartAndEndDates = true)
    {
        HashSet<T> results = [];
        if (IncludeStartAndEndDates == true)
        {
            results.Add(StartDate);
            results.Add(EndDate);
        }

        T EvaluationDateTime = GetNextTimeStep(StartDate);
        while (EvaluationDateTime < EndDate)
        {
            results.Add(EvaluationDateTime);
            EvaluationDateTime = GetNextTimeStep(EvaluationDateTime);
        }
        return results;
    }

    /// <summary>
    /// Calculates the next time step based on the periodicity configuration.
    /// </summary>
    /// <param name="SelectedDateTime"></param>
    /// <returns></returns>
    public T GetNextTimeStep(T SelectedDateTime) => CalculateTimeStep(SelectedDateTime, true);

    /// <summary>
    /// Calculates the previous time step based on the periodicity configuration.
    /// </summary>
    /// <param name="SelectedDateTime"></param>
    /// <returns></returns>
    public T GetPreviousTimeStep(T SelectedDateTime) => CalculateTimeStep(SelectedDateTime, false);

    /// <summary>
    /// Checks if the SelectedDateTime aligns with the defined time steps.
    /// Said another way, this method checks if the SelectedDateTime is a valid time step.
    /// </summary>
    /// <param name="SelectedDateTime"></param>
    /// <returns></returns>
    public bool IsTimeStepAligned(T SelectedDateTime)
    {
        T nextDateTime = GetNextTimeStep(SelectedDateTime);
        T previousDateTime = GetPreviousTimeStep(nextDateTime);
        return SelectedDateTime == previousDateTime;
    }

    /// <summary>
    /// Calculates the next or previous time step based on the periodicity configuration.
    /// </summary>
    /// <param name="SelectedDateTime"></param>
    /// <param name="step_forward"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    private T CalculateTimeStep(T SelectedDateTime, bool step_forward)
    {
        T result = Configuration. PeriodicityConfig.Periodicity switch
        {
            Periodicity.Daily => IsIntraday() ? GetNextIntradayDateTime(SelectedDateTime, step_forward) :
                AdjustDate(SelectedDateTime, step_forward),
            Periodicity.Monthly => GetMonthEndDateTime(SelectedDateTime, step_forward),
            Periodicity.Weekly => GetWeekEndDateTime(SelectedDateTime, step_forward),
            Periodicity.Quarterly => GetQuarterEndDateTime(SelectedDateTime, step_forward),
            Periodicity.Annually => GetYearEndDateTime(SelectedDateTime, step_forward),
            Periodicity.SemiAnnual => GetSemiAnnualEndDateTime(SelectedDateTime, step_forward),
            _ => throw new NotImplementedException(),
        };
        return result;
    }

    private bool IsIntraday() => Configuration.PeriodicityConfig.Periodicity == Periodicity.Daily
        && Configuration.PeriodicityConfig.Time is not null;

    private T GetNextIntradayDateTime(T SelectedDateTime, bool step_forward)
    {
        if (TimeSpanStepper is null)
            throw new InvalidOperationException("TimeSpanStepper is not initialized for intraday calculations.");

        T result = step_forward ? TimeSpanStepper.GetNext(SelectedDateTime)
            : TimeSpanStepper.GetPrevious(SelectedDateTime);

        // We adjust based on the step direction rather than the global adjustment direction. This makes more sense for intraday steps as we always want to move in the direction of travel not the user preference.
        return AdjustDateIfNeeded(result, step_forward);
    }


    private T GetWeekEndDateTime(T SelectedDateTime, bool MoveForward)
    {
        if (Configuration.RequiredDayOfWeek is null) throw new InvalidOperationException("RequiredDayOfWeek must be set for weekly periodicity.");

        T new_date = MoveForward ?
            SelectedDateTime.NextDayOfWeekSpecified(Configuration.RequiredDayOfWeek.Value)
            : SelectedDateTime.PreviousDayOfWeekSpecified(Configuration.RequiredDayOfWeek.Value);
        return AdjustDateIfNeeded(new_date, MoveForward);
    }

    private T GetMonthEndDateTime(T SelectedDateTime, bool MoveForward)
    {
        DateTime eom_date = SelectedDateTime.DateTime.EndOfMonth();

        (bool found, T new_date) = FindNextValidDate(SelectedDateTime, MoveForward, eom_date);
        if (found) return new_date;

        int months_to_add = MoveForward ? 1 : -1;
        new_date = eom_date.AddMonths(months_to_add).EndOfMonth();
        return AdjustDateIfNeeded(new_date, MoveForward);
    }

    private T GetQuarterEndDateTime(T SelectedDateTime, bool MoveForward)
    {
        DateTime eoq_date = SelectedDateTime.DateTime.EndOfQuarter();
       
        (bool found, T new_date) = FindNextValidDate(SelectedDateTime, MoveForward, eoq_date);
        if (found) return new_date;

        int months_to_add = MoveForward ? 3 : -3;
        new_date = eoq_date.AddMonths(months_to_add).EndOfQuarter();
        return AdjustDateIfNeeded(new_date, MoveForward);
    }

    private T GetSemiAnnualEndDateTime(T SelectedDateTime, bool MoveForward)
    {
        DateTime eos_date = SelectedDateTime.DateTime.EndOfHalfYear();
       
        (bool found, T new_date) = FindNextValidDate(SelectedDateTime, MoveForward, eos_date);
        if (found) return new_date;

        int months_to_add = (MoveForward ? 6 : -6);
        new_date = eos_date.AddMonths(months_to_add).EndOfHalfYear();
        return AdjustDateIfNeeded(new_date, MoveForward);
    }

    private T GetYearEndDateTime(T SelectedDateTime, bool MoveForward)
    {
        DateTime eoy_date = SelectedDateTime.DateTime.EndOfYear();

        (bool found, T new_date) = FindNextValidDate(SelectedDateTime, MoveForward, eoy_date);
        if (found) return new_date;

        int years_to_add = MoveForward ? 1 : -1;
        new_date = new_date.AddYears(years_to_add);
        return AdjustDateIfNeeded(new_date, MoveForward);
    }

    /// <summary>
    /// Checks if a valid date is found based on the movement direction and selected date.
    /// If valid, returns the valid, new date.
    /// </summary>
    /// <param name="SelectedDateTime"></param>
    /// <param name="MoveForwardInTime"></param>
    /// <param name="new_date"></param>
    /// <returns></returns>
    private (bool found, T value) FindNextValidDate(T SelectedDateTime, bool MoveForwardInTime, DateTime new_date)
    {
        new_date = AdjustDateIfNeeded(new_date, MoveForwardInTime);
        return MoveForwardInTime switch
        {
            true
                when new_date > SelectedDateTime.DateTime => (found: true, value: new_date),
            false
                when new_date < SelectedDateTime.DateTime => (found: true, value: new_date),
            _ => (found: false, value: new_date),
        };
    }

    /// <summary>
    /// Adjusts the SelectedDateTime until it matches the required day type as per PeriodicityConfig.
    /// </summary>
    /// <param name="SelectedDateTime"></param>
    /// <param name="moving_forward_in_time"></param>
    /// <returns></returns>
    private T AdjustDateIfNeeded(T SelectedDateTime, bool moving_forward_in_time)
    {
        return IsValidDayOfWeek(SelectedDateTime) 
            ? SelectedDateTime
            // Use provided adjust_forward if not null, otherwise use the class-level AdjustmentDirectionForward
            : AdjustDate(SelectedDateTime, moving_forward_in_time); 
    }

    /// <summary>
    /// Adjusts the SelectedDateTime to the next valid date based on the DaySelectionType in PeriodicityConfig.
    /// </summary>
    /// <param name="SelectedDateTime"></param>
    /// <param name="MovingForwardInTime"></param>
    /// <returns></returns>
    /// <exception cref="NotSupportedException"></exception>
    private T AdjustDate(T SelectedDateTime, bool MovingForwardInTime)
    {
        int days_to_add = GetAdjustmentDirectionScalar(MovingForwardInTime);
        return Configuration.PeriodicityConfig.DayType switch
        {
            DaySelectionType.Any => SelectedDateTime.AddDays(days_to_add),
            DaySelectionType.Weekday => SelectedDateTime.AddWeekdays(days_to_add),
            DaySelectionType.Weekend => SelectedDateTime.AddWeekendDays(days_to_add),
            // null-forgiving operator is safe here due to validation in constructor
            DaySelectionType.SpecificDayOfWeek => Configuration.AdjustmentType == DateAdjustmentType.MoveForward
                ? SelectedDateTime.NextDayOfWeekSpecified(Configuration.RequiredDayOfWeek!.Value)
                : SelectedDateTime.PreviousDayOfWeekSpecified(Configuration.RequiredDayOfWeek!.Value), 
            _ => throw new NotSupportedException($"Day selection type {Configuration.PeriodicityConfig.DayType} is not supported."),
        };
    }

    /// <summary>
    /// Checks if the SelectedDateTime matches the required day type as per PeriodicityConfig.
    /// </summary>
    /// <param name="SelectedDateTime"></param>
    /// <returns></returns>
    /// <exception cref="NotSupportedException"></exception>
    private bool IsValidDayOfWeek(T SelectedDateTime)
    {
        return Configuration.PeriodicityConfig.DayType switch
        {
            DaySelectionType.Any => true,
            DaySelectionType.Weekday => SelectedDateTime.DayOfWeek.IsWeekday(),
            DaySelectionType.Weekend => SelectedDateTime.DayOfWeek.IsWeekend(),
            DaySelectionType.SpecificDayOfWeek => SelectedDateTime.DayOfWeek == Configuration.RequiredDayOfWeek,
            _ => throw new NotSupportedException($"Day selection type {Configuration.PeriodicityConfig.DayType} is not supported."),
        };
    }
}
