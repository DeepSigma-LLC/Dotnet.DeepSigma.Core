using Xunit;
using DeepSigma.Core.DateTimeUnification;

namespace DeepSigma.Core.Tests.Tests;


public class TimeIntervalStepper
{
    [Fact]
    public void Test_DateTime_Stepper_Min_5()
    {
        var stepper = new TimeStepper.TimeSpanStepper<DateTimeCustom>(Enums.TimeInterval.Min_5);
        DateTimeCustom dt1 = new DateTime(2025, 1, 1);
        DateTimeCustom next1 = stepper.GetNext(dt1);
        DateTimeCustom expected1 = new DateTime(2025, 1, 1, hour: 0, minute: 5, second: 0);
        Assert.Equal(expected1, next1);

        DateTimeCustom next2 = stepper.GetNext(next1);
        DateTimeCustom expected2 = new DateTime(2025, 1, 1, hour: 0, minute: 10, second: 0);
        Assert.Equal(expected2, next2);

        DateTimeCustom next3 = stepper.GetNext(next2);
        DateTimeCustom expected3 = new DateTime(2025, 1, 1, hour: 0, minute: 15, second: 0);
        Assert.Equal(expected3, next3);
    }

    [Fact]
    public void Test_DateTime_Stepper_Min_15_Previous()
    {
        var stepper = new TimeStepper.TimeSpanStepper<DateTimeCustom>(Enums.TimeInterval.Min_15);

        DateTimeCustom dt1 = new DateTime(2025, 1, 1, hour: 1, minute: 0, second: 0);
        DateTimeCustom prev1 = stepper.GetPrevious(dt1);
        DateTimeCustom expected1 = new DateTime(2025, 1, 1, hour: 0, minute: 45, second: 0);
        Assert.Equal(expected1, prev1);

        DateTimeCustom prev2 = stepper.GetPrevious(prev1);
        DateTimeCustom expected2 = new DateTime(2025, 1, 1, hour: 0, minute: 30, second: 0);
        Assert.Equal(expected2, prev2);

        DateTimeCustom prev3 = stepper.GetPrevious(prev2);
        DateTimeCustom expected3 = new DateTime(2025, 1, 1, hour: 0, minute: 15, second: 0);
        Assert.Equal(expected3, prev3);
    }


    [Fact]
    public void Test_DateTime_Stepper_Min_30_NextAndPrevious()
    {
        var stepper = new TimeStepper.TimeSpanStepper<DateTimeCustom>(Enums.TimeInterval.Min_30);
        DateTimeCustom dt1 = new DateTime(2025, 1, 1, hour: 2, minute: 0, second: 0);

        DateTimeCustom next1 = stepper.GetNext(dt1);
        DateTimeCustom expectedNext1 = new DateTime(2025, 1, 1, hour: 2, minute: 30, second: 0);
        Assert.Equal(expectedNext1, next1);

        DateTimeCustom prev1 = stepper.GetPrevious(next1);
        DateTimeCustom expectedPrev1 = new DateTime(2025, 1, 1, hour: 2, minute: 0, second: 0);
        Assert.Equal(expectedPrev1, prev1);
    }


    [Fact]
    public void Test_DateTime_Stepper_Over_Day_Min_60()
    {
        var stepper = new TimeStepper.TimeSpanStepper<DateTimeCustom>(Enums.TimeInterval.Min_60);
        DateTimeCustom dt1 = new DateTime(2025, 1, 1);

        DateTimeCustom previous1 = stepper.GetPrevious(dt1);
        DateTimeCustom expected1 = new DateTime(2024, 12, 31, hour: 23, minute: 0, second: 0);
        Assert.Equal(expected1, previous1);

        DateTimeCustom previous2 = stepper.GetPrevious(previous1);
        DateTimeCustom expected2 = new DateTime(2024, 12, 31, hour: 22, minute: 00, second: 0);
        Assert.Equal(expected2, previous2);

        DateTimeCustom next1 = stepper.GetNext(previous2);
        DateTimeCustom expected3 = new DateTime(2024, 12, 31, hour: 23, minute: 0, second: 0);
        Assert.Equal(expected3, next1);

        DateTimeCustom next2 = stepper.GetNext(next1);
        DateTimeCustom expected4 = new DateTime(2025, 1, 1, hour: 0, minute: 0, second: 0);
        Assert.Equal(expected4, next2);
    }

}
