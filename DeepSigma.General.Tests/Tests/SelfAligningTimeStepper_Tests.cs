using Xunit;
using DeepSigma.Core.TimeStepper;
using DeepSigma.Core.DateTimeUnification;
using DeepSigma.Core.Extensions;

namespace DeepSigma.Core.Tests.Tests;

public class SelfAligningTimeStepper_Tests
{
    [Fact]
    public void Test_Monthly_Steps()
    {
        PeriodicityConfiguration configuration = new(Enums.Periodicity.Monthly, Enums.DaySelectionType.Any);
        TimeStepperConfiguration timeStepperConfiguration = new(configuration, DateAdjustmentType.MoveBackward);
        
        SelfAligningTimeStepper<DateTimeCustom> stepper = new(timeStepperConfiguration);

        DateTimeCustom expected = new DateTime(2024, 3, 31);
        DateTimeCustom next1 = stepper.GetNextTimeStep(DateTimeCustom.Parse("2024-03-15"));
        Assert.Equal(expected, next1);

        DateTimeCustom expected2 = new DateTime(2024, 4, 30);
        DateTimeCustom next2 = stepper.GetNextTimeStep(next1);
        Assert.Equal(expected2, next2);

        DateTimeCustom expected3 = new DateTime(2024, 5, 31);
        DateTimeCustom next3 = stepper.GetNextTimeStep(next2);
        Assert.Equal(expected3, next3);

        DateTimeCustom expected4 = new DateTime(2024, 6, 30);
        DateTimeCustom next4 = stepper.GetNextTimeStep(next3);
        Assert.Equal(expected4, next4);

        DateTimeCustom expected5 = new DateTime(2024, 7, 31);
        DateTimeCustom next5 = stepper.GetNextTimeStep(next4);
        Assert.Equal(expected5, next5);
    }


    [Fact]
    public void Test_Annual_Steps()
    {
        PeriodicityConfiguration configuration = new(Enums.Periodicity.Annually, Enums.DaySelectionType.Any);
        TimeStepperConfiguration timeStepperConfiguration = new(configuration, DateAdjustmentType.MoveBackward);
        SelfAligningTimeStepper<DateTimeCustom> stepper = new(timeStepperConfiguration);

        DateTimeCustom expected = new DateTime(2024, 12, 31);
        DateTimeCustom next1 = stepper.GetNextTimeStep(DateTimeCustom.Parse("2024-03-15"));
        Assert.Equal(expected, next1);

        DateTimeCustom expected2 = new DateTime(2025, 12, 31);
        DateTimeCustom next2 = stepper.GetNextTimeStep(next1);
        Assert.Equal(expected2, next2);

        DateTimeCustom expected3 = new DateTime(2026, 12, 31);
        DateTimeCustom next3 = stepper.GetNextTimeStep(next2);
        Assert.Equal(expected3, next3);

        DateTimeCustom expected4 = new DateTime(2027, 12, 31);
        DateTimeCustom next4 = stepper.GetNextTimeStep(next3);
        Assert.Equal(expected4, next4);

        DateTimeCustom expected5 = new DateTime(2028, 12, 31);
        DateTimeCustom next5 = stepper.GetNextTimeStep(next4);
        Assert.Equal(expected5, next5);
    }


    [Fact]
    public void Test_SemiAnnual_Steps()
    {
        PeriodicityConfiguration configuration = new(Enums.Periodicity.SemiAnnual, Enums.DaySelectionType.Any);
        TimeStepperConfiguration timeStepperConfiguration = new(configuration, DateAdjustmentType.MoveBackward);
        SelfAligningTimeStepper<DateTimeCustom> stepper = new(timeStepperConfiguration);

        DateTimeCustom expected = new DateTime(2024, 6, 30);
        DateTimeCustom next1 = stepper.GetNextTimeStep(DateTimeCustom.Parse("2024-03-15"));
        Assert.Equal(expected, next1);

        DateTimeCustom expected2 = new DateTime(2024, 12, 31);
        DateTimeCustom next2 = stepper.GetNextTimeStep(next1);
        Assert.Equal(expected2, next2);

        DateTimeCustom expected3 = new DateTime(2025, 6, 30);
        DateTimeCustom next3 = stepper.GetNextTimeStep(next2);
        Assert.Equal(expected3, next3);

        DateTimeCustom expected4 = new DateTime(2025, 12, 31);
        DateTimeCustom next4 = stepper.GetNextTimeStep(next3);
        Assert.Equal(expected4, next4);

        DateTimeCustom expected5 = new DateTime(2026, 6, 30);
        DateTimeCustom next5 = stepper.GetNextTimeStep(next4);
        Assert.Equal(expected5, next5);
    }


    [Fact]
    public void Test_Weekly_Steps()
    {
        PeriodicityConfiguration configuration = new(Enums.Periodicity.Weekly, Enums.DaySelectionType.Any);
        TimeStepperConfiguration timeStepperConfiguration = new(configuration, DateAdjustmentType.MoveBackward, DayOfWeek.Friday);
        SelfAligningTimeStepper<DateTimeCustom> stepper = new(timeStepperConfiguration);

        DateTimeCustom expected = new DateTime(2025, 12, 5);
        DateTimeCustom next1 = stepper.GetNextTimeStep(DateTimeCustom.Parse("2025-12-01"));
        Assert.Equal(expected, next1);

        DateTimeCustom expected2 = new DateTime(2025, 12, 12);
        DateTimeCustom next2 = stepper.GetNextTimeStep(next1);
        Assert.Equal(expected2, next2);

        DateTimeCustom expected3 = new DateTime(2025, 12, 19);
        DateTimeCustom next3 = stepper.GetNextTimeStep(next2);
        Assert.Equal(expected3, next3);

        DateTimeCustom expected4 = new DateTime(2025, 12, 26);
        DateTimeCustom next4 = stepper.GetNextTimeStep(next3);
        Assert.Equal(expected4, next4);

        DateTimeCustom expected5 = new DateTime(2026, 1, 2);
        DateTimeCustom next5 = stepper.GetNextTimeStep(next4);
        Assert.Equal(expected5, next5);
    }

    [Fact]
    public void Test_Daily_Steps()
    {
        PeriodicityConfiguration configuration = new(Enums.Periodicity.Daily, Enums.DaySelectionType.Any);
        TimeStepperConfiguration timeStepperConfiguration = new(configuration, DateAdjustmentType.MoveInDirectionOfTimeStep);
        SelfAligningTimeStepper<DateTimeCustom> stepper = new(timeStepperConfiguration);

        DateTimeCustom expected = new DateTime(2025, 12, 2);
        DateTimeCustom next1 = stepper.GetNextTimeStep(DateTimeCustom.Parse("2025-12-01"));
        Assert.Equal(expected, next1);

        DateTimeCustom expected2 = new DateTime(2025, 12, 3);
        DateTimeCustom next2 = stepper.GetNextTimeStep(next1);
        Assert.Equal(expected2, next2);

        DateTimeCustom expected3 = new DateTime(2025, 12, 4);
        DateTimeCustom next3 = stepper.GetNextTimeStep(next2);
        Assert.Equal(expected3, next3);

        DateTimeCustom expected4 = new DateTime(2025, 12, 5);
        DateTimeCustom next4 = stepper.GetNextTimeStep(next3);
        Assert.Equal(expected4, next4);

        DateTimeCustom expected5 = new DateTime(2025, 12, 6);
        DateTimeCustom next5 = stepper.GetNextTimeStep(next4);
        Assert.Equal(expected5, next5);
    }

    [Fact]
    public void Test_Daily_WeekdaysOnly_Steps()
    {
        PeriodicityConfiguration configuration = new(Enums.Periodicity.Daily, Enums.DaySelectionType.Weekday);
        TimeStepperConfiguration timeStepperConfiguration = new(configuration, DateAdjustmentType.MoveInDirectionOfTimeStep);
        SelfAligningTimeStepper<DateTimeCustom> stepper = new(timeStepperConfiguration);

        DateTimeCustom expected = new DateTime(2025, 12, 2);
        DateTimeCustom next1 = stepper.GetNextTimeStep(DateTimeCustom.Parse("2025-12-01"));
        Assert.Equal(expected, next1);

        DateTimeCustom expected2 = new DateTime(2025, 12, 3);
        DateTimeCustom next2 = stepper.GetNextTimeStep(next1);
        Assert.Equal(expected2, next2);

        DateTimeCustom expected3 = new DateTime(2025, 12, 4);
        DateTimeCustom next3 = stepper.GetNextTimeStep(next2);
        Assert.Equal(expected3, next3);

        DateTimeCustom expected4 = new DateTime(2025, 12, 5);
        DateTimeCustom next4 = stepper.GetNextTimeStep(next3);
        Assert.Equal(expected4, next4);

        DateTimeCustom expected5 = new DateTime(2025, 12, 8);
        DateTimeCustom next5 = stepper.GetNextTimeStep(next4);
        Assert.Equal(expected5, next5);
    }

    [Fact]
    public void Test_Daily_Intraday()
    {
        PeriodicityConfiguration configuration = new(Enums.Periodicity.Daily, Enums.DaySelectionType.Any, Enums.TimeInterval.Min_30);
        TimeStepperConfiguration timeStepperConfiguration = new(configuration, DateAdjustmentType.MoveInDirectionOfTimeStep);
        SelfAligningTimeStepper<DateTimeCustom> stepper = new(timeStepperConfiguration);

        DateTimeCustom expected = new DateTime(2025, 12, 1, hour:0, minute:30, second:0);
        DateTimeCustom next1 = stepper.GetNextTimeStep(DateTimeCustom.Parse("2025-12-01"));
        Assert.Equal(expected, next1);

        DateTimeCustom expected2 = new DateTime(2025, 12, 1, hour:1, minute: 0, second: 0);
        DateTimeCustom next2 = stepper.GetNextTimeStep(next1);
        Assert.Equal(expected2, next2);

        DateTimeCustom expected3 = new DateTime(2025, 12, 1, hour: 1, minute: 30, second: 0);
        DateTimeCustom next3 = stepper.GetNextTimeStep(next2);
        Assert.Equal(expected3, next3);

        DateTimeCustom expected4 = new DateTime(2025, 12, 1, hour: 2, minute: 0, second: 0);
        DateTimeCustom next4 = stepper.GetNextTimeStep(next3);
        Assert.Equal(expected4, next4);

        DateTimeCustom expected5 = new DateTime(2025, 12, 1, hour: 2, minute: 30, second: 0);
        DateTimeCustom next5 = stepper.GetNextTimeStep(next4);
        Assert.Equal(expected5, next5);
    }

    [Fact]
    public void Test_Daily_Intraday_WeekdaysOnly()
    {
        PeriodicityConfiguration configuration = new(Enums.Periodicity.Daily, Enums.DaySelectionType.Weekday, Enums.TimeInterval.Min_30);
        TimeStepperConfiguration timeStepperConfiguration = new(configuration, DateAdjustmentType.MoveInDirectionOfTimeStep);
        SelfAligningTimeStepper<DateTimeCustom> stepper = new(timeStepperConfiguration);

        DateTimeCustom expected = new DateTime(2025, 12, 5, hour: 23, minute: 30, second: 0);
        DateTimeCustom next1 = stepper.GetNextTimeStep(new DateTime(2025, 12, 5, hour: 23, minute: 0, second: 0));
        Assert.Equal(expected, next1);

        DateTimeCustom expected2 = new DateTime(2025, 12, 8, hour: 0, minute: 0, second: 0); // Next weekday after weekend
        DateTimeCustom next2 = stepper.GetNextTimeStep(next1);
        Assert.Equal(expected2, next2);

        DateTimeCustom expected3 = new DateTime(2025, 12, 8, hour: 0, minute: 30, second: 0);
        DateTimeCustom next3 = stepper.GetNextTimeStep(next2);
        Assert.Equal(expected3, next3);

        DateTimeCustom expected4 = new DateTime(2025, 12, 8, hour: 1, minute: 0, second: 0);
        DateTimeCustom next4 = stepper.GetNextTimeStep(next3);
        Assert.Equal(expected4, next4);

        DateTimeCustom expected5 = new DateTime(2025, 12, 8, hour: 1, minute: 30, second: 0);
        DateTimeCustom next5 = stepper.GetNextTimeStep(next4);
        Assert.Equal(expected5, next5);
    }


    [Fact]
    public void Test_Daily_WeekdaysOnly_Steps_WithAdjustmentDirectionBackward_Should_Still_MoveForward()
    {
        PeriodicityConfiguration configuration = new(Enums.Periodicity.Daily, Enums.DaySelectionType.Weekday);
        TimeStepperConfiguration timeStepperConfiguration = new(configuration, DateAdjustmentType.MoveInDirectionOfTimeStep);
        SelfAligningTimeStepper<DateTimeCustom> stepper = new(timeStepperConfiguration);

        DateTimeCustom expected = new DateTime(2025, 12, 2);
        DateTimeCustom next1 = stepper.GetNextTimeStep(DateTimeCustom.Parse("2025-12-01"));
        Assert.Equal(expected, next1);

        DateTimeCustom expected2 = new DateTime(2025, 12, 3);
        DateTimeCustom next2 = stepper.GetNextTimeStep(next1);
        Assert.Equal(expected2, next2);

        DateTimeCustom expected3 = new DateTime(2025, 12, 4);
        DateTimeCustom next3 = stepper.GetNextTimeStep(next2);
        Assert.Equal(expected3, next3);

        DateTimeCustom expected4 = new DateTime(2025, 12, 5);
        DateTimeCustom next4 = stepper.GetNextTimeStep(next3);
        Assert.Equal(expected4, next4);

        DateTimeCustom expected5 = new DateTime(2025, 12, 8);
        DateTimeCustom next5 = stepper.GetNextTimeStep(next4);
        Assert.Equal(expected5, next5);
    }


    [Fact]
    public void Test_Weekly_Steps_Target_Thursday()
    {
        PeriodicityConfiguration configuration = new(Enums.Periodicity.Weekly, Enums.DaySelectionType.SpecificDayOfWeek);
        TimeStepperConfiguration timeStepperConfiguration = new(configuration, DateAdjustmentType.MoveBackward, DayOfWeek.Thursday);
        SelfAligningTimeStepper<DateTimeCustom> stepper = new(timeStepperConfiguration);

        DateTimeCustom expected = new DateTime(2025, 12, 4);
        DateTimeCustom next1 = stepper.GetNextTimeStep(DateTimeCustom.Parse("2025-12-01"));
        Assert.Equal(expected, next1);

        DateTimeCustom expected2 = new DateTime(2025, 12, 11);
        DateTimeCustom next2 = stepper.GetNextTimeStep(next1);
        Assert.Equal(expected2, next2);

        DateTimeCustom expected3 = new DateTime(2025, 12, 18);
        DateTimeCustom next3 = stepper.GetNextTimeStep(next2);
        Assert.Equal(expected3, next3);

        DateTimeCustom expected4 = new DateTime(2025, 12, 25);
        DateTimeCustom next4 = stepper.GetNextTimeStep(next3);
        Assert.Equal(expected4, next4);

        DateTimeCustom expected5 = new DateTime(2026, 1, 1);
        DateTimeCustom next5 = stepper.GetNextTimeStep(next4);
        Assert.Equal(expected5, next5);
    }


    [Fact]
    public void Test_Monthly_Steps_WeekdaysOnly()
    {
        PeriodicityConfiguration configuration = new(Enums.Periodicity.Monthly, Enums.DaySelectionType.Weekday);
        TimeStepperConfiguration timeStepperConfiguration = new(configuration, DateAdjustmentType.MoveBackward);
        SelfAligningTimeStepper<DateTimeCustom> stepper = new(timeStepperConfiguration);

        DateTimeCustom expected = new DateTime(2024, 3, 31).WeekdayOrPrevious();
        DateTimeCustom next1 = stepper.GetNextTimeStep(DateTimeCustom.Parse("2024-03-15"));
        Assert.Equal(expected, next1);

        DateTimeCustom expected2 = new DateTime(2024, 4, 30).WeekdayOrPrevious();
        DateTimeCustom next2 = stepper.GetNextTimeStep(next1);
        Assert.Equal(expected2, next2);

        DateTimeCustom expected3 = new DateTime(2024, 5, 31).WeekdayOrPrevious();
        DateTimeCustom next3 = stepper.GetNextTimeStep(next2);
        Assert.Equal(expected3, next3);

        DateTimeCustom expected4 = new DateTime(2024, 6, 30).WeekdayOrPrevious();
        DateTimeCustom next4 = stepper.GetNextTimeStep(next3);
        Assert.Equal(expected4, next4);

        DateTimeCustom expected5 = new DateTime(2024, 7, 31).WeekdayOrPrevious();
        DateTimeCustom next5 = stepper.GetNextTimeStep(next4);
        Assert.Equal(expected5, next5);
    }

    [Fact]
    public void Test_Monthly_Steps_WeekendsOnly()
    {
        PeriodicityConfiguration configuration = new(Enums.Periodicity.Monthly, Enums.DaySelectionType.Weekend);
        TimeStepperConfiguration timeStepperConfiguration = new(configuration, DateAdjustmentType.MoveBackward);
        SelfAligningTimeStepper<DateTimeCustom> stepper = new(timeStepperConfiguration);

        DateTimeCustom expected = new DateTime(2024, 3, 31).WeekendOrPrevious();
        DateTimeCustom next1 = stepper.GetNextTimeStep(DateTimeCustom.Parse("2024-03-15"));
        Assert.Equal(expected, next1);

        DateTimeCustom expected2 = new DateTime(2024, 4, 30).WeekendOrPrevious();
        DateTimeCustom next2 = stepper.GetNextTimeStep(next1);
        Assert.Equal(expected2, next2);

        DateTimeCustom expected3 = new DateTime(2024, 5, 31).WeekendOrPrevious();
        DateTimeCustom next3 = stepper.GetNextTimeStep(next2);
        Assert.Equal(expected3, next3);

        DateTimeCustom expected4 = new DateTime(2024, 6, 30).WeekendOrPrevious();
        DateTimeCustom next4 = stepper.GetNextTimeStep(next3);
        Assert.Equal(expected4, next4);

        DateTimeCustom expected5 = new DateTime(2024, 7, 31).WeekendOrPrevious();
        DateTimeCustom next5 = stepper.GetNextTimeStep(next4);
        Assert.Equal(expected5, next5);
    }

    [Fact]
    public void Test_Monthly_Steps_WeekendsOnly_AdjustForward()
    {
        PeriodicityConfiguration configuration = new(Enums.Periodicity.Monthly, Enums.DaySelectionType.Weekend);
        TimeStepperConfiguration timeStepperConfiguration = new(configuration, DateAdjustmentType.MoveBackward);
        SelfAligningTimeStepper<DateTimeCustom> stepper = new(timeStepperConfiguration);

        DateTimeCustom expected = new DateTime(2024, 3, 31).WeekendOrNext();
        DateTimeCustom next1 = stepper.GetNextTimeStep(DateTimeCustom.Parse("2024-03-15"));
        Assert.Equal(expected, next1);

        DateTimeCustom expected2 = new DateTime(2024, 4, 30).WeekendOrNext();
        DateTimeCustom next2 = stepper.GetNextTimeStep(next1);
        Assert.Equal(expected2, next2);

        DateTimeCustom expected3 = new DateTime(2024, 5, 31).WeekendOrNext();
        DateTimeCustom next3 = stepper.GetNextTimeStep(next2);
        Assert.Equal(expected3, next3);

        DateTimeCustom expected4 = new DateTime(2024, 6, 30).WeekendOrNext();
        DateTimeCustom next4 = stepper.GetNextTimeStep(next3);
        Assert.Equal(expected4, next4);

        DateTimeCustom expected5 = new DateTime(2024, 7, 31).WeekendOrNext();
        DateTimeCustom next5 = stepper.GetNextTimeStep(next4);
        Assert.Equal(expected5, next5);
    }
}
