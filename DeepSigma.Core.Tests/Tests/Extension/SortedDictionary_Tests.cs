using DeepSigma.Core.DateTimeUnification;
using DeepSigma.Core.Extensions;
using DeepSigma.Core.TimeStepper;
using Xunit;

namespace DeepSigma.Core.Tests.Tests.Extension;

public class SortedDictionary_Tests
{
    [Fact]
    public void Test_RemoveNullValues()
    {
        // Arrange
        SortedDictionary<DateOnlyCustom, decimal?> dict = new()
        {
            { new DateOnlyCustom(2025, 12, 10), 100.0m },
            { new DateOnlyCustom(2025, 12, 11), null },
            { new DateOnlyCustom(2025, 12, 12), 200.0m },
            { new DateOnlyCustom(2025, 12, 13), null },
            { new DateOnlyCustom(2025, 12, 14), 300.0m }
        };
        // Act
        var result = dict.RemoveNulls();
        // Assert
        Assert.Equal(3, result.Count);
        Assert.True(result.ContainsKey(new DateOnlyCustom(2025, 12, 10)));
        Assert.True(result.ContainsKey(new DateOnlyCustom(2025, 12, 12)));
        Assert.True(result.ContainsKey(new DateOnlyCustom(2025, 12, 14)));
    }

    [Fact]
    public void RemoveInvalidDates()
    {

        // Arrange
        SortedDictionary<DateOnlyCustom, decimal?> dict = new()
        {
            { new DateOnlyCustom(2025, 12, 10), 100.0m },
            { new DateOnlyCustom(2025, 12, 11), null },
            { new DateOnlyCustom(2025, 12, 12), 200.0m },
            { new DateOnlyCustom(2025, 12, 13), null }, // Saturday
            { new DateOnlyCustom(2025, 12, 14), 300.0m }, // Sunday
            { new DateOnlyCustom(2025, 12, 15), null  },
            { new DateOnlyCustom(2025, 12, 16), null  }
        };

        TimeStepperConfiguration configuration = new(new PeriodicityConfiguration(Enums.Periodicity.Daily, Enums.DaySelectionType.Weekday));
        SelfAligningTimeStepper<DateOnlyCustom> timeStepper = new(configuration);

        // Act
        var result = dict.RemoveInvalidDates(timeStepper);
        // Assert
        Assert.Equal(5, result.Count);
        Assert.True(result.ContainsKey(new DateOnlyCustom(2025, 12, 10)));
        Assert.True(result.ContainsKey(new DateOnlyCustom(2025, 12, 11)));
        Assert.True(result.ContainsKey(new DateOnlyCustom(2025, 12, 12)));
        Assert.True(result.ContainsKey(new DateOnlyCustom(2025, 12, 15)));
        Assert.True(result.ContainsKey(new DateOnlyCustom(2025, 12, 16)));
    }


    [Fact]
    public void Test_SortedDictionary_Roll()
    {
        // Arrange
        SortedDictionary<DateOnlyCustom, decimal?> dict = new()
        {
            { new DateOnlyCustom(2025, 12, 10), 100.0m },
            { new DateOnlyCustom(2025, 12, 11), 200.0m },
            { new DateOnlyCustom(2025, 12, 30), 300.0m }
        };

        TimeStepperConfiguration configuration = new(new PeriodicityConfiguration(Enums.Periodicity.Daily, Enums.DaySelectionType.Any));
        var results = dict.FillMissingValuesByRolling(new SelfAligningTimeStepper<DateOnlyCustom>(configuration));

        Assert.Equal(21, results.Count);
        Assert.Equal(100.0m, results[new DateOnlyCustom(2025, 12, 10)]);
        Assert.Equal(200.0m, results[new DateOnlyCustom(2025, 12, 11)]);
        Assert.Equal(200.0m, results[new DateOnlyCustom(2025, 12, 12)]);
        Assert.Equal(200.0m, results[new DateOnlyCustom(2025, 12, 13)]);
        Assert.Equal(200.0m, results[new DateOnlyCustom(2025, 12, 14)]);
        Assert.Equal(200.0m, results[new DateOnlyCustom(2025, 12, 15)]);
        Assert.Equal(300.0m, results[new DateOnlyCustom(2025, 12, 30)]);
    }

    [Fact]
    public void Test_SortedDictionary_FillWithNull()
    {
        // Arrange
        SortedDictionary<DateOnlyCustom, decimal?> dict = new()
        {
            { new DateOnlyCustom(2025, 12, 10), 100.0m },
            { new DateOnlyCustom(2025, 12, 11), 200.0m },
            { new DateOnlyCustom(2025, 12, 30), 300.0m }
        };

        TimeStepperConfiguration configuration = new(new PeriodicityConfiguration(Enums.Periodicity.Daily, Enums.DaySelectionType.Any));
        var results = dict.FillMissingValuesWithNull(new SelfAligningTimeStepper<DateOnlyCustom>(configuration));

        Assert.Equal(21, results.Count);
        Assert.Equal(100.0m, results[new DateOnlyCustom(2025, 12, 10)]);
        Assert.Equal(200.0m, results[new DateOnlyCustom(2025, 12, 11)]);
        Assert.Null(results[new DateOnlyCustom(2025, 12, 12)]);
        Assert.Null(results[new DateOnlyCustom(2025, 12, 13)]);
        Assert.Null(results[new DateOnlyCustom(2025, 12, 14)]);
        Assert.Null(results[new DateOnlyCustom(2025, 12, 15)]);
        Assert.Equal(300.0m, results[new DateOnlyCustom(2025, 12, 30)]);
    }
}
