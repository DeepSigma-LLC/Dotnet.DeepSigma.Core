using System.Collections.Concurrent;

namespace DeepSigma.Core.Concurrency;

/// <summary>
/// Enables robust parellel method execution along with asyncronous progress reporting.
/// </summary>
/// <typeparam name="TMethodInput"></typeparam>
/// <typeparam name="TResult"></typeparam>
internal class ParellelTaskExecutionEngine<TMethodInput, TResult>
{
    internal TaskCollection Tasks { get; set; } = new();
    private Progress<TaskCollection> ProgressReport { get; set; } = new Progress<TaskCollection>();

    /// <summary>
    /// Runs defined methods in parallel.
    /// </summary>
    /// <param name="unit_of_work_method"></param>
    /// <param name="method_parameters"></param>
    /// <param name="cancellation_token"></param>
    /// <returns></returns>
    public async Task<List<ThreadMethodResult<TResult>>> RunParallelAsync(Func<TMethodInput, ThreadMethodResult<TResult>> unit_of_work_method,
        List<TMethodInput> method_parameters, CancellationToken cancellation_token)
    {
        Tasks = new();
        Tasks.MessageLog.Enqueue("Parallel task execution starting now...");
        List<ThreadMethodResult<TResult>> results = [];
        try
        {
            results = await RunParallelTasks(unit_of_work_method, method_parameters, cancellation_token);
        }
        catch (OperationCanceledException)
        {
            Tasks.MessageLog.Enqueue("Parallel task execution was cancelled.");
            TriggerProgressReportEvent();
        }
        return results;
    }

    /// <summary>
    /// Subscribe to progress events by passing a method of type "void ReportProgress(object? sender, ProgressReportDataModel e)"
    /// </summary>
    /// <param name="report_progress_event"></param>
    public void SubscribeToProgressReportEvent(EventHandler<TaskCollection> report_progress_event)
    {
        ProgressReport.ProgressChanged += report_progress_event;
    }

    /// <summary>
    /// Runs the parallel tasks using the provided method and parameters.
    /// </summary>
    /// <param name="unit_of_work_method"></param>
    /// <param name="method_parameters"></param>
    /// <param name="cancellation_token"></param>
    /// <returns></returns>
    private async Task<List<ThreadMethodResult<TResult>>> RunParallelTasks(Func<TMethodInput, ThreadMethodResult<TResult>> unit_of_work_method,
        List<TMethodInput> method_parameters, CancellationToken cancellation_token)
    {
        ConcurrentQueue<ThreadMethodResult<TResult>> TaskOutputResults = [];
        await Task.Run(cancellationToken: cancellation_token, action:() =>
        {
            Parallel.ForEach(method_parameters, parameters =>
            {
                ThreadStatusDataModel taskProgressDataModel = new();
                taskProgressDataModel.TaskStart();
                AddTaskToProgressReport(Tasks, taskProgressDataModel);
                TriggerProgressReportEvent();

                ThreadMethodResult<TResult> result = unit_of_work_method(parameters); //Method executioin

                SaveResults(TaskOutputResults, result, taskProgressDataModel);
                taskProgressDataModel.TaskComplete();
                TriggerProgressReportEvent();

                if (cancellation_token.IsCancellationRequested)
                {
                    cancellation_token.ThrowIfCancellationRequested();
                }
            });
        });
        PostProcessingLogMessageUpdate();
        return TaskOutputResults.ToList();
    }

    /// <summary>
    /// Saves the results of the task execution.
    /// </summary>
    /// <param name="result_set"></param>
    /// <param name="results"></param>
    /// <param name="thread_status_model"></param>
    private static void SaveResults(ConcurrentQueue<ThreadMethodResult<TResult>> result_set, ThreadMethodResult<TResult> results, ThreadStatusDataModel thread_status_model)
    {
        result_set.Enqueue(results);
        foreach (var message in results.MessageLog)
        {
            thread_status_model.TaskMessageLog.Enqueue(message);
        }
    }

    /// <summary>
    /// Adds task to progress report.
    /// </summary>
    /// <param name="task_collection"></param>
    /// <param name="thread_status_model"></param>
    private static void AddTaskToProgressReport(TaskCollection task_collection, ThreadStatusDataModel thread_status_model)
    {
        task_collection.ThreadStatusDataModels.Enqueue(thread_status_model);
    }

    /// <summary>
    /// Logs status messages to the log.
    /// </summary>
    private void PostProcessingLogMessageUpdate()
    {
        Tasks.MessageLog.Enqueue("Task execution is now complete!");
        int completeTasks = Tasks.ThreadStatusDataModels.Where(x => x.IsTaskComplete).Count();
        double totalTasks = Tasks.GetTotalTaskCount();
        Tasks.MessageLog.Enqueue($"{completeTasks} out of {totalTasks} tasks were completed. ({completeTasks / totalTasks * 100}%)");
    }

    /// <summary>
    /// Triggers methods subscribed to the reporting event.
    /// </summary>
    private void TriggerProgressReportEvent()
    {
        ((IProgress<TaskCollection>)ProgressReport).Report(Tasks);
    }
}
