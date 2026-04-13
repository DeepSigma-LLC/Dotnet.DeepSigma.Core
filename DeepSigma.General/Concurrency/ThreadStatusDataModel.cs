using System.Collections.Concurrent;
using System.Diagnostics;

namespace DeepSigma.Core.Concurrency;

/// <summary>
/// Data model for tracking the status of a thread's execution.
/// </summary>
public class ThreadStatusDataModel
{
    /// <summary>
    /// Unique identifier for the task instance.
    /// </summary>
    public Guid Guid { get; } = Guid.NewGuid();
    /// <summary>
    /// Indicates whether the task has started.
    /// </summary>
    public bool IsTaskStarted { get; private set; } = false;
    /// <summary>
    /// Indicates whether the task is currently running.
    /// </summary>
    public bool IsTaskRunning { get; private set; } = false;

    /// <summary>
    /// Indicates whether the task has been completed.
    /// </summary>
    public bool IsTaskComplete { get; private set; } = false;

    /// <summary>
    /// Start date and time of the task execution.
    /// </summary>
    public DateTime? StartDateTime { get; private set; }

    /// <summary>
    /// End date and time of the task execution.
    /// </summary>
    public DateTime? EndDateTime { get; private set; }

    /// <summary>
    /// Managed thread ID of the task execution.
    /// </summary>
    public int ExecutionManagedThreadId { get; private set; }

    /// <summary>
    /// Name of the current thread executing the task.
    /// </summary>
    public string? CurrentThreadName { get; private set; }

    /// <summary>
    /// Current processor ID where the task is running.
    /// </summary>
    public int CurrentProcessorId { get; private set; }

    /// <summary>
    /// Queue to log messages related to the task execution.
    /// </summary>
    public ConcurrentQueue<string> TaskMessageLog { get; set; } = [];

    private Stopwatch _taskStopWatch = new();

    private Thread _thread;

    /// <summary>
    /// Initializes task and class properties for current task execution.
    /// </summary>
    public void TaskStart()
    {
        StartDateTime = DateTime.Now;
        _thread = Thread.CurrentThread;
        ExecutionManagedThreadId = Thread.CurrentThread.ManagedThreadId;
        CurrentThreadName = Thread.CurrentThread.Name;
        CurrentProcessorId = Thread.GetCurrentProcessorId();
        IsTaskStarted = true;
        IsTaskRunning = true;
        _taskStopWatch.Start();
    }

    /// <summary>
    /// Mark current task as complete.
    /// </summary>
    public void TaskComplete()
    {
        IsTaskComplete = true;
        TaskStop();
    }

    /// <summary>
    /// Stops task.
    /// </summary>
    public void TaskStop()
    {
        _taskStopWatch.Stop();
        EndDateTime = DateTime.Now;
        IsTaskRunning = false;
    }

    /// <summary>
    /// Returns task current run time.
    /// </summary>
    /// <returns></returns>
    public long GetRunTime()
    {
        //Start then start stopwatch in order for ElapsedTime to be updated.
        _taskStopWatch.Stop();
        _taskStopWatch.Start();
        return _taskStopWatch.ElapsedMilliseconds;
    }
}
