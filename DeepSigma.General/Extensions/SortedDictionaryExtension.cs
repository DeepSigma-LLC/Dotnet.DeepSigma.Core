using DeepSigma.Core.DateTimeUnification;
using DeepSigma.Core.Enums;
using DeepSigma.Core.TimeStepper;

namespace DeepSigma.Core.Extensions;

/// <summary>
/// Provides extension methods for SortedDictionary.
/// </summary>
public static class SortedDictionaryExtension
{

    /// <summary>
    /// Gets a windowed series with a specified method applied over the observation window.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="Data"> The data series to process.</param>
    /// <param name="Transform"> The method to apply over the observation window.</param>
    /// <param name="ObservationWindowCount"> The size of the observation window.</param>
    /// <param name="fillSelector"> A function to provide fill values for initial entries where the window is not full.</param>
    /// <returns></returns>
    public static SortedDictionary<TKey, TValue> GetWindowedSeriesWithMethodApplied<TKey, TValue>(this SortedDictionary<TKey, TValue> Data, Func<IEnumerable<TValue>, TValue> Transform, int ObservationWindowCount = 20, Func<TValue>? fillSelector = null)
        where TKey : notnull, IComparable<TKey>
    {
        SortedDictionary<TKey, TValue> results = [];
        fillSelector ??= () => default;

        // Fill initial values with nulls
        for (int i = 0; i < ObservationWindowCount - 1; i++)
        {
            results.Add(Data.FirstOrDefault().Key, fillSelector());
        }

        for (int i_to_skip = 0; i_to_skip <= Data.Count - ObservationWindowCount; i_to_skip++)
        {
            IEnumerable<TValue> subset = Data.Skip(i_to_skip).Take(ObservationWindowCount).Select(x => x.Value);
            TValue result = Transform(subset);
            int index = ObservationWindowCount + i_to_skip - 1;
            results.Add(Data.ElementAt(index).Key, result);
        }
        return results;
    }

    /// <summary>
    /// Gets an expanding windowed series with a specified method applied over the observation window.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="Data"></param>
    /// <param name="Transform"> The method to apply over the expanding window.</param>
    /// <returns></returns>
    public static SortedDictionary<TKey, TValue> GetExpandingWindowedSeriesWithMethodApplied<TKey, TValue>(this SortedDictionary<TKey, TValue> Data, Func<IEnumerable<TValue>, TValue> Transform)
    where TKey : notnull, IComparable<TKey>
    {
        SortedDictionary<TKey, TValue> results = [];
        for (int i = 0; i <= Data.Count - 1; i++)
        {
            IEnumerable<TValue> subset = Data.Take(i + 1).Select(x => x.Value);
            TValue result = Transform(subset);
            results.Add(Data.ElementAt(i).Key, result);
        }
        return results;
    }

    /// <summary>
    /// Gets a series with a specified method applied to each point value.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="Data"></param>
    /// <param name="Transform"></param>
    /// <returns></returns>
    public static SortedDictionary<TKey, TValue> GetSeriesWithMethodApplied<TKey, TValue>(this SortedDictionary<TKey, TValue> Data, Func<TValue, TValue> Transform)
    where TKey : notnull, IComparable<TKey>
    {
        SortedDictionary<TKey, TValue> results = [];
        foreach(var item in Data)
        {
            TValue result = Transform(item.Value);
            results.Add(item.Key, result);
        }
        return results;
    }

    /// <summary>
    /// Removed invalid dates from dictionary. Dates that do not 
    /// </summary>
    /// <typeparam name="TDate"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="Data"></param>
    /// <param name="TimeStep"></param>
    /// <returns></returns>
    public static SortedDictionary<TDate, TValue> RemoveInvalidDates<TDate, TValue>(this SortedDictionary<TDate, TValue> Data, SelfAligningTimeStepper<TDate> TimeStep)
        where TDate : struct, IDateTime<TDate>
    {
        SortedDictionary<TDate, TValue> results = [];
        TDate StartDate = Data.Keys.Min();
        TDate EndDate = Data.Keys.Max();
        TDate selectedDateTime = TimeStep.IsTimeStepAligned(StartDate) ? StartDate : TimeStep.GetNextTimeStep(StartDate);

        while (selectedDateTime <= EndDate)
        {
            bool found = Data.TryGetValue(selectedDateTime, out TValue? value);
            if (found) results.Add(selectedDateTime, value!); // only add if found therefore safe to suppress null warning. Will only be null if TValue is nullable type.
            selectedDateTime = TimeStep.GetNextTimeStep(selectedDateTime);
        }
        return results;
    }

    /// <summary>
    /// Rolls forward the last known value to fill missing dates in the time series. Required dates determined from periodicity time stepper.
    /// </summary>
    /// <param name="Data"></param>
    /// <param name="TimeStep"></param>
    /// <returns></returns>
    public static SortedDictionary<TDate, TValue> FillMissingValuesByRollingAndDropExcess<TDate, TValue>(this SortedDictionary<TDate, TValue> Data, SelfAligningTimeStepper<TDate> TimeStep)
        where TDate : struct, IDateTime<TDate>
    {
        SortedDictionary<TDate, TValue> results = [];
        TDate StartDate = Data.Keys.Min();
        TDate EndDate = Data.Keys.Max();
        TDate selectedDateTime = TimeStep.IsTimeStepAligned(StartDate) ? StartDate : TimeStep.GetNextTimeStep(StartDate);
        TValue lastKnownValue = Data[StartDate];

        while (selectedDateTime <= EndDate)
        {
            bool found = Data.TryGetValue(selectedDateTime, out TValue? new_value);
            if (found) lastKnownValue = new_value!; // only update if found therefore safe to suppress null warning. Will only be null if TValue is nullable type.

            results.Add(selectedDateTime, lastKnownValue);
            selectedDateTime = TimeStep.GetNextTimeStep(selectedDateTime);
        }
        return results;
    }

    /// <summary>
    /// Rolls forward the last known value to fill missing dates in the time series. Time steps are determined by the provided time stepper.
    /// Retains any excess dates that were originally in the data.
    /// </summary>
    /// <typeparam name="TDate"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="Data"></param>
    /// <param name="TimeStep"></param>
    /// <returns></returns>
    public static SortedDictionary<TDate, TValue> FillMissingValuesByRolling<TDate, TValue>(this SortedDictionary<TDate, TValue> Data, SelfAligningTimeStepper<TDate> TimeStep)
    where TDate : struct, IDateTime<TDate>
    {
        SortedDictionary<TDate, TValue> results = Data.FillMissingValuesByRollingAndDropExcess(TimeStep);

        // Now retain any excess dates that were originally in the data
        Data.ForEach(x => results.TryAdd(x.Key, x.Value));
        return results;
    }

    /// <summary>
    /// Fills missing dates required by the time step with null if no data exists for that date.
    /// </summary>
    /// <param name="Data"></param>
    /// <param name="TimeStep"></param>
    /// <returns></returns>
    public static SortedDictionary<TDate, TValue?> FillMissingValuesWithNullAndDropExcess<TDate, TValue>(this SortedDictionary<TDate, TValue> Data, SelfAligningTimeStepper<TDate> TimeStep)
        where TDate : struct, IDateTime<TDate>
    {
        SortedDictionary<TDate, TValue?> results = [];
        TDate StartDate = Data.Keys.Min();
        TDate EndDate = Data.Keys.Max();
        TDate selectedDateTime = TimeStep.IsTimeStepAligned(StartDate) ? StartDate : TimeStep.GetNextTimeStep(StartDate);
        while (selectedDateTime <= EndDate)
        {
            bool found = Data.TryGetValue(selectedDateTime, out TValue? value);
            results.Add(selectedDateTime, value); // value will be null if not found
            selectedDateTime = TimeStep.GetNextTimeStep(selectedDateTime);
        }
        return results;
    }

    /// <summary>
    /// Fills missing dates required by the time step with null if no data exists for that date. Time steps are determined by the provided time stepper.
    /// Retains any excess dates that were originally in the data.
    /// </summary>
    /// <typeparam name="TDate"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="Data"></param>
    /// <param name="TimeStep"></param>
    /// <returns></returns>
    public static SortedDictionary<TDate, TValue?> FillMissingValuesWithNull<TDate, TValue>(this SortedDictionary<TDate, TValue> Data, SelfAligningTimeStepper<TDate> TimeStep)
        where TDate : struct, IDateTime<TDate>
    {
        SortedDictionary<TDate, TValue?> results = Data.FillMissingValuesWithNullAndDropExcess(TimeStep);

        // Now retain any excess dates that were originally in the data
        Data.ForEach(x => results.TryAdd(x.Key, x.Value));
        return results;
    }

    /// <summary>
    /// Gets lagged time series.
    /// </summary>
    /// <param name="Data"></param>
    /// <param name="DaysToLag"></param>
    /// <param name="daySelection"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public static SortedDictionary<TDate, TValue> LagByDays<TDate, TValue>(this SortedDictionary<TDate, TValue> Data, int DaysToLag, DaySelectionType daySelection = DaySelectionType.Any)
        where TDate : struct, IDateTime<TDate>
    {
        return daySelection switch
        {
            (DaySelectionType.Any) => AddDaysToDateTimes(Data, -DaysToLag),
            (DaySelectionType.Weekday) => AddBusinessDaysToDateTimes(Data, -DaysToLag),
            (DaySelectionType.Weekend) => AddWeekendDaysToDateTimes(Data, -DaysToLag),
            _ => throw new NotImplementedException(),
        };
    }

    /// <summary>
    /// Adds days to time series date times.
    /// </summary>
    /// <param name="Data"></param>
    /// <param name="DaysToAdd"></param>
    /// <returns></returns>
    private static SortedDictionary<TDate, TValue> AddDaysToDateTimes<TDate, TValue>(SortedDictionary<TDate, TValue> Data, int DaysToAdd)
        where TDate : struct, IDateTime<TDate>
    {
        return DaysToAdd == 0 ? Data :
            Data.ToDictionary(x => x.Key.AddDays(DaysToAdd), x => x.Value).ToSortedDictionary();
    }

    /// <summary>
    /// Adds business days to time series date times.
    /// </summary>
    /// <param name="Data"></param>
    /// <param name="BusinessDaysToAdd"></param>
    /// <returns></returns>
    private static SortedDictionary<TDate, TValue> AddBusinessDaysToDateTimes<TDate, TValue>(SortedDictionary<TDate, TValue> Data, int BusinessDaysToAdd)
        where TDate : struct, IDateTime<TDate>
    {
        return BusinessDaysToAdd == 0 ? Data :
             Data.ToDictionary(x => x.Key.AddWeekdays(BusinessDaysToAdd), x => x.Value).ToSortedDictionary();
    }

    /// <summary>
    /// Adds business days to time series date times.
    /// </summary>
    /// <param name="Data"></param>
    /// <param name="WeekendDaysToAdd"></param>
    /// <returns></returns>
    private static SortedDictionary<TDate, TValue> AddWeekendDaysToDateTimes<TDate, TValue>(SortedDictionary<TDate, TValue> Data, int WeekendDaysToAdd)
        where TDate : struct, IDateTime<TDate>
    {
        return WeekendDaysToAdd == 0 ? Data :
             Data.ToDictionary(x => x.Key.AddWeekendDays(WeekendDaysToAdd), x => x.Value).ToSortedDictionary();
    }

}
