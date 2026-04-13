using DeepSigma.Core.Extensions;

namespace DeepSigma.Core.Enums;


/// <summary>
/// Utilities for working with dates and periodicities.
/// </summary>
public static class PeriodicityUtilities
{
    /// <summary>
    /// Get the annualization multiplier based on a selected periodicity.
    /// </summary>
    /// <param name="DateTimes"></param>
    /// <returns></returns>
    public static decimal GetAnnualizationMultiplier(DateTime[] DateTimes)
    {
        Periodicity estimatedPeriodicity = GetEstimatedPeriodicityUsingFuzzyLogic(DateTimes);
        bool IncludeWeekends = DoesDailyAnnualizationIncludeWeekends(DateTimes);
        return Math.Sqrt(GetPeriodsPerYear(estimatedPeriodicity, IncludeWeekends).ToDecimal());
    }

    private static bool DoesDailyAnnualizationIncludeWeekends(DateTime[] DateTimes)
    {
        TimeSpan averageTimeSpanDifference = CalculateAverageTimeDifference(DateTimes);
        return averageTimeSpanDifference.Days == 1;
    }

    /// <summary>
    /// Gets periods per selected periodicity.
    /// </summary>
    /// <param name="periodicity"></param>
    /// <param name="include_weekends"></param>
    /// <param name="days_in_year"></param>
    /// <param name="trading_days_in_year"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public static int GetPeriodsPerYear(Periodicity periodicity, bool include_weekends = false, int days_in_year = 365, int trading_days_in_year = 255)
    {
        switch (periodicity)
        {
            case (Periodicity.Annually):
                return 1;
            case (Periodicity.SemiAnnual):
                return 2;
            case (Periodicity.Quarterly):
                return 4;
            case (Periodicity.Weekly):
                return 52;
            case (Periodicity.Daily):
                if (include_weekends == true) return days_in_year;
                return trading_days_in_year;
            default:
                throw new NotImplementedException();
        }
    }
    /// <summary>
    /// Returns the smallest periodicity from collection of periodicities.
    /// </summary>
    /// <param name="Periodicities"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public static Periodicity GetSmallestTimeStepPeriodicity(HashSet<Periodicity> Periodicities)
    {
        //Must be ordered
        Periodicity[] periodicitiesSmallestToLarest = { Periodicity.Daily, Periodicity.Weekly,
                Periodicity.Monthly, Periodicity.Quarterly, Periodicity.SemiAnnual, Periodicity.Annually};
        foreach (Periodicity selectedPeriodicity in periodicitiesSmallestToLarest)
        {
            if (Periodicities.Contains(selectedPeriodicity) == true) return selectedPeriodicity;
        }
        throw new NotImplementedException();
    }

    /// <summary>
    /// Gets the smallest time interval from a collection of time intervals.
    /// </summary>
    /// <param name="TimeIntervals"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public static TimeInterval GetSmallestTimeStepTimeInterval(HashSet<TimeInterval> TimeIntervals)
    {
        //Must be ordered
        TimeInterval[] timeIntervalSmallestToLarest = { TimeInterval.Min_1, TimeInterval.Min_2, TimeInterval.Min_3, TimeInterval.Min_5,
            TimeInterval.Min_10, TimeInterval.Min_15, TimeInterval.Min_20, TimeInterval.Min_30, TimeInterval.Min_45, TimeInterval.Min_60,
            TimeInterval.Min_90, TimeInterval.Min_120, TimeInterval.Min_180};
        foreach (TimeInterval selectedTimeInterval in timeIntervalSmallestToLarest)
        {
            if (TimeIntervals.Contains(selectedTimeInterval) == true)  return selectedTimeInterval;
        }
        throw new NotImplementedException();
    }


    /// <summary>
    /// Gets estimaated periodicity from an array of date times using fuzzy logic.
    /// </summary>
    /// <param name="DateTimes"></param>
    /// <returns></returns>
    public static Periodicity GetEstimatedPeriodicityUsingFuzzyLogic(DateTime[] DateTimes)
    {
        TimeSpan averageDifference = CalculateAverageTimeDifference(DateTimes);
        int DaysPerMonthLowEstimate = 25;
        if (averageDifference.Days >= DaysPerMonthLowEstimate * 12)
        {
            return Periodicity.Annually;
        }
        else if (averageDifference.Days >= DaysPerMonthLowEstimate * 6)
        {
            return Periodicity.SemiAnnual;
        }
        else if (averageDifference.Days >= DaysPerMonthLowEstimate * 3)
        {
            return Periodicity.Quarterly;
        }
        else if (averageDifference.Days >= DaysPerMonthLowEstimate)
        {
            return Periodicity.Monthly;
        }
        else if (averageDifference.Days >= 4)
        {
            return Periodicity.Weekly;
        }
        else
        {
            return Periodicity.Daily;
        }
    }

    private static TimeSpan CalculateAverageTimeDifference(DateTime[] DateTimes)
    {
        TimeSpan[] totalDifferences = CalculateTimeDifferences(DateTimes);
        TimeSpan totalDifference = totalDifferences.Aggregate((total, current) => total + current);
        TimeSpan averageDifference = new(totalDifference.Ticks / totalDifferences.Length);
        return averageDifference;
    }

    private static TimeSpan[] CalculateTimeDifferences(DateTime[] dates)
    {
        TimeSpan[] timeDifferences = new TimeSpan[dates.Length - 1];
        for (int i = 0; i < dates.Length - 1; i++)
        {
            timeDifferences[i] = dates[i + 1] - dates[i];
        }
        return timeDifferences;
    }
}
