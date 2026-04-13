using DeepSigma.Core.DateTimeUnification;
using DeepSigma.Core.Enums;

namespace DeepSigma.Core.TimeStepper;

/// <summary>
/// Provides functionality to step through <see cref="IDateTime{T}"/> values based on a specified TimeSpan interval.
/// </summary>
/// <remarks>
/// For example, you can create a <see cref="TimeSpanStepper{T}"/> with a 15-minute interval and use it to get the next or previous time steps from a given <see cref="IDateTime{T}"/> value.
/// Alternatively, you can retrieve all time steps between two <see cref="IDateTime{T}"/> values either including or excluding the bounds in the case where you want to generate a series of time points at regular intervals.
/// The class supports backward and forward stepping through time and the ability to handle different time intervals specified via <see cref="TimeInterval"/> enum or custom <see cref="TimeSpan"/> values which allows for flexible time manipulation and custom time steps as needed.
/// For example, you can create a stepper with a 21-minute and 13 second interval by using the <see cref="TimeSpanStepper{T}(TimeSpan)"/> constructor.
/// If you need to work with different date periodicities such as daily, weekly, monthly, quaterly, semi-annual, or annual intervals, consider using the <see cref="SelfAligningTimeStepper{T}"/> which is designed to handle such scenarios.
/// </remarks>
public class TimeSpanStepper<T> 
    where T : struct, IDateTime<T>
{
    private readonly TimeSpan _time_span_step;

    /// <inheritdoc cref="TimeSpanStepper{T}"/>
    public TimeSpanStepper(TimeInterval time_interval)
    {
        TimeSpan span = time_interval.ToTimeSpan();
        Validate(span);
        _time_span_step = span;
    }

    /// <inheritdoc cref="TimeSpanStepper{T}"/>
    public TimeSpanStepper(int days = 0, int Hours = 0, int Minutes = 0, int Seconds = 0, int Milliseconds = 0)
    {
        TimeSpan step = new(days, Hours, Minutes, Seconds, Milliseconds);
        Validate(step);
        _time_span_step = step;
    }

    /// <inheritdoc cref="TimeSpanStepper{T}"/>
    public TimeSpanStepper(TimeSpan time_span_step)
    {
        Validate(time_span_step);
        _time_span_step = time_span_step;
    }

    
    private void Validate(TimeSpan span)
    {
        if (span.TotalMilliseconds == 0) throw new InvalidOperationException("Time step must be greater than zero.");
    }

    /// <summary>
    /// Get the next step from the given DateTime.
    /// </summary>
    /// <param name="from"></param>
    /// <returns></returns>
    public T GetNext(T from)
    {
        double total_minutes = from.DateTime.TimeOfDay.TotalMinutes;
        double step_minutes = _time_span_step.TotalMinutes;

        double completed_steps = total_minutes / step_minutes;
        completed_steps = Math.Ceiling(completed_steps);

        double target_steps = completed_steps + 1;
        return from.DateTime.Date.AddMinutes(target_steps * step_minutes);
    }


    /// <summary>
    /// Get the previous step from the given DateTime.
    /// </summary>
    /// <param name="from"></param>
    /// <returns></returns>
    public T GetPrevious(T from)
    {
        double total_minutes = from.DateTime.TimeOfDay.TotalMinutes;
        double step_minutes = _time_span_step.TotalMinutes;

        double completed_steps = total_minutes / step_minutes;
        completed_steps = Math.Ceiling(completed_steps);

        double remainer = total_minutes % step_minutes;
        double target_steps = remainer == 0 ?
             completed_steps - 1
            : completed_steps;
        return from.DateTime.Date.AddMinutes(target_steps * step_minutes);
    }

    /// <summary>
    /// Get steps between two DateTime values including bounds (from_date and end_date).
    /// </summary>
    /// <param name="from_date"></param>
    /// <param name="end_date"></param>
    /// <returns></returns>
    public IEnumerable<T> GetStepsIncludingBounds(T from_date, T end_date)
    {
        HashSet<T> results = [];
        results.Add(from_date);
        results.Add(end_date);

        IEnumerable<T> steps = GetSteps(from_date, end_date);
        foreach (T step in steps)
        {
            results.Add(step);
        }
        return results;
    }

    /// <summary>
    /// Get steps between two DateTime values excluding bounds (from_date and end_date).
    /// </summary>
    /// <param name="from_date"></param>
    /// <param name="end_date"></param>
    /// <returns></returns>
    public IEnumerable<T> GetStepsExcludingBounds(T from_date, T end_date)
    {
        List<T> results = [];
        IEnumerable<T> steps = GetSteps(from_date, end_date);
        foreach (T step in steps)
        {
            if (step != from_date && step != end_date)
            {
                results.Add(step);
            }
        }
        return results;
    }

    /// <summary>
    /// Get steps between two DateTime values (from_date inclusive, end_date inclusive assuming the step matches).
    /// </summary>
    /// <param name="from_date"></param>
    /// <param name="end_date"></param>
    /// <returns></returns>
    public IEnumerable<T> GetSteps(T from_date, T end_date)
    {
        return from_date < end_date ?
            GetStepsForward(from_date, end_date)
            : GetStepsBackward(from_date, end_date);
    }

    /// <summary>
    /// Get steps forward between two DateTime values (from_date inclusive, end_date inclusive assuming the step matches).
    /// </summary>
    /// <param name="from_date"></param>
    /// <param name="end_date"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    private IEnumerable<T> GetStepsForward(T from_date, T end_date)
    {
        if (from_date > end_date) throw new ArgumentException("From date must be less than to date.");

        T current = from_date;
        while (current <= end_date)
        {
            yield return current;
            current = GetNext(current);
        }
    }

    /// <summary>
    /// Get steps backward between two DateTime values (from_date inclusive, end_date inclusive assuming the step matches).
    /// </summary>
    /// <param name="from_date"></param>
    /// <param name="end_date"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    private IEnumerable<T> GetStepsBackward(T from_date, T end_date)
    {
        if (from_date < end_date) throw new ArgumentException("From date must be greater than to date.");

        T current = from_date;
        while (current >= end_date)
        {
            yield return current;
            current = GetPrevious(current);
        }
    }
}
