namespace MyThreadPool;

/// <summary>
/// class represents a single operation and that executes async in different threads.
/// </summary>
/// <typeparam name="TResult">operation result type.</typeparam>
public interface IMyTask<TResult>
{
    /// <summary>
    /// Gets true if task completed.
    /// </summary>
    public bool IsCompleted { get; }
    
    /// <summary>
    /// Gets result of task.
    /// </summary>
    public TResult Result { get; }

    /// <summary>
    /// apply method to the result of this task.
    /// </summary>
    /// <param name="continueMethod">method to be applied to the result of current task</param>
    /// <typeparam name="TNewResult">result type of <see cref="continueMethod"/></typeparam>
    /// <returns>result of method <see cref="continueMethod"/> with current task result in argument</returns>
    public IMyTask<TNewResult> ContinueWith<TNewResult>(Func<TResult, TNewResult> continueMethod);
}