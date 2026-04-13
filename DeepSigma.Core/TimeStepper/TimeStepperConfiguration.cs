
using DeepSigma.Core.Enums;
using System.Diagnostics.CodeAnalysis;

namespace DeepSigma.Core.TimeStepper;

/// <summary>
/// Configuration for time stepping operations.
/// </summary>
public class TimeStepperConfiguration
{
    /// <summary>
    /// Configuration defining the periodicity of the time steps.
    /// </summary>
    public required PeriodicityConfiguration PeriodicityConfig {  get; init; }

    /// <summary>
    /// Defines how to adjust dates that do not align with the specified periodicity.
    /// </summary>
    public DateAdjustmentType AdjustmentType { get; init; } = DateAdjustmentType.MoveInDirectionOfTimeStep;

    /// <summary>
    /// Gets the required day of the week for weekly periodicity.
    /// </summary>
    public DayOfWeek? RequiredDayOfWeek
    {
        get => field;
        init => field =
            (PeriodicityConfig.DayType == DaySelectionType.SpecificDayOfWeek || PeriodicityConfig.Periodicity == Periodicity.Weekly)
            ? value
            : null; // Ensures value is only set when DayType is SpecificDayOfWeek or Periodicity is Weekly
    }

    /// <inheritdoc cref="TimeStepperConfiguration"/>
    public TimeStepperConfiguration(){ }

    /// <inheritdoc cref="TimeStepperConfiguration"/>
    [SetsRequiredMembers]
    public TimeStepperConfiguration(PeriodicityConfiguration periodicityConfiguration)
    {
        this.PeriodicityConfig = periodicityConfiguration;
    }

    /// <inheritdoc cref="TimeStepperConfiguration"/>
    [SetsRequiredMembers]
    public TimeStepperConfiguration(PeriodicityConfiguration periodicityConfiguration, DateAdjustmentType adjustmentType = DateAdjustmentType.MoveInDirectionOfTimeStep, DayOfWeek? requiredDayOfWeek = null)
    {
        this.PeriodicityConfig = periodicityConfiguration;
        this.AdjustmentType = adjustmentType;
        this.RequiredDayOfWeek = requiredDayOfWeek;
    }
}
