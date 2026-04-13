using System.Collections.Concurrent;

namespace DeepSigma.Core.Concurrency;

/// <summary>
/// Represents a collection of tasks.
/// </summary>
public class TaskCollection
{
    /// <summary>
    /// A collection of messages logged during task execution.
    /// </summary>
    public ConcurrentQueue<string> MessageLog { get; set; } = [];

    /// <summary>
    /// A collection of data models containing thread status information.
    /// </summary>
    public ConcurrentQueue<ThreadStatusDataModel> ThreadStatusDataModels { get; set; } = [];

    /// <summary>
    /// Returns the percentage of tasks that are complete in the collection.
    /// </summary>
    /// <returns></returns>
    public double GetPercentageComplete()
    {
        double completedTasks = ThreadStatusDataModels.Where(x => x.IsTaskComplete == true).Count();
        return completedTasks / GetTotalTaskCount() * 100;
    }

    /// <summary>
    /// Returns the total number of tasks in the collection.
    /// </summary>
    /// <returns></returns>
    public double GetTotalTaskCount() => ThreadStatusDataModels.Count;
}