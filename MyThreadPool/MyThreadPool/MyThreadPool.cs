using System.Collections.Concurrent;

namespace MyThreadPool;

/// <summary>
/// Class which provides a pool of threads that can be used to execute tasks
/// </summary>
public class MyThreadPool
{
    private readonly Semaphore threadRunController = new(0, int.MaxValue);
    private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
    private readonly ConcurrentQueue<Action> tasksQueue = new();
    private readonly MyThread[] threads;
    private readonly ManualResetEvent shutdownEvent = new(true);
    private readonly object lockObject = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="MyThreadPool"/> class.
    /// </summary>
    /// <param name="threadsNumber">number of threads in ThreadPool</param>
    /// <exception cref="IndexOutOfRangeException">threads number must to be more than 0</exception>
    public MyThreadPool(int threadsNumber)
    {
        if (threadsNumber <= 0)
        {
            throw new IndexOutOfRangeException("threads number must to be more than 0");
        }

        threads = Enumerable.Range(0, threadsNumber)
                .Select(_ => new MyThread(cancellationTokenSource.Token, this.tasksQueue, this.threadRunController)).ToArray();
    }
    
    /// <summary>
    /// Gets number of working threads.
    /// </summary>
    public int WorkingThreadsNumber
    {
        get => threads.Count(thread => thread.IsWorking);
    }

    /// <summary>
    /// Queues a method for execution. The method executes when a thread pool thread becomes available.
    /// </summary>
    /// <param name="method">method for execution.</param>
    /// <typeparam name="TResult">Result type of method.</typeparam>
    /// <returns>Instance of <see cref="IMyTask{TResult}"/></returns>
    /// <exception cref="OperationCanceledException">Throws on attempt of submitting after shutdown</exception>
    public IMyTask<TResult> Submit<TResult>(Func<TResult> method)
    {
        ArgumentNullException.ThrowIfNull(method);

        var newTask = new MyTask<TResult>(method, cancellationTokenSource.Token, this);
        
        Submit(newTask.Evaluate);
        
        return newTask;
    }
    
    private void Submit(Action task)
    {
        ArgumentNullException.ThrowIfNull(task);

        shutdownEvent.WaitOne();

        lock (lockObject)
        {
            cancellationTokenSource.Token.ThrowIfCancellationRequested();
            tasksQueue.Enqueue(task);
            threadRunController.Release();
        }
    }

    /// <summary>
    /// Shut down the threads. Already running tasks are not interrupted,
    /// but new tasks are not accepted for execution by threads from the pool.
    /// </summary>
    public void Shutdown()
    {
        shutdownEvent.WaitOne();

        shutdownEvent.Reset();

        if (cancellationTokenSource.IsCancellationRequested)
        {
            return;
        }

        cancellationTokenSource.Cancel();
        threadRunController.Release(threads.Length + 1);

        foreach (var thread in threads)
        {
            thread.Join();
        }

        shutdownEvent.Set();
    }

    /// <summary>
    /// Thread abstract for Thread, initially for checking it's state
    /// </summary>
    private class MyThread
    {
        private readonly CancellationToken cancellationToken;
        private readonly Semaphore threadRunController;
        private readonly ConcurrentQueue<Action> tasksQueue;
        private readonly Thread thread;

        /// <summary>
        /// Initializes a new instance of the <see cref="MyThread"/> class.
        /// </summary>
        /// <param name="cancellationToken">cancellation token.</param>
        /// <param name="tasksQueue">queue of tasks for executing.</param>
        /// <param name="threadRunController">Semaphore, which release on submitting of a new task.</param>
        public MyThread(
            CancellationToken cancellationToken,
            ConcurrentQueue<Action> tasksQueue,
            Semaphore threadRunController)
        {
            this.cancellationToken = cancellationToken;
            this.tasksQueue = tasksQueue;
            this.threadRunController = threadRunController;
            thread = CreateThread();
            thread.Start();
        }

        /// <summary>
        /// Gets a value indicating whether thread is working.
        /// </summary>
        public bool IsWorking { get; private set; }

        /// <summary>
        /// Thread join.
        /// </summary>
        public void Join() => thread.Join();

        private Thread CreateThread() 
            => new(() =>
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    threadRunController.WaitOne();

                    if (tasksQueue.TryDequeue(out var task) && !cancellationToken.IsCancellationRequested)
                    {
                        IsWorking = true;
                        task.Invoke();
                        IsWorking = false;
                    }
                }
            });
    }

    /// <inheritdoc cref="IMyTask{TResult}"/>
    private class MyTask<TResult> : IMyTask<TResult>
    {
        private readonly Func<TResult> method;
        private readonly ManualResetEvent resultWaitingEvent = new(false);
        private readonly CancellationToken cancellationToken;
        private readonly List<Action> continuationTasks = new();
        private readonly object continuationBlockObject = new();
        private readonly MyThreadPool threadPool;
        private Exception? thrownException;
        private TResult? result;

        /// <summary>
        /// Initializes a new instance of the <see cref="MyTask{TResult}"/> class.
        /// </summary>
        /// <param name="method">method for execution</param>
        /// <param name="cancellationToken">cancellation token</param>
        /// <param name="threadPool">parental thread pool.</param>
        /// <exception cref="ArgumentNullException">method can't be null</exception>
        public MyTask(
            Func<TResult> method,
            CancellationToken cancellationToken,
            MyThreadPool threadPool)
        {
            ArgumentNullException.ThrowIfNull(method);

            this.cancellationToken = cancellationToken;
            this.threadPool = threadPool;
            this.method = method;
        }

        /// <summary>
        /// Gets a value indicating completion of task.
        /// </summary>
        public bool IsCompleted { get; private set; }

        /// <summary>
        /// Gets result of task execution.
        /// </summary>
        /// <exception cref="AggregateException">throw if task method thrown exception.</exception>
        /// <exception cref="InvalidOperationException">Unexpected behaviour.</exception>
        /// <exception cref="OperationCanceledException">shutdown requested.</exception>
        public TResult Result
        {
            get
            {
                if (!IsCompleted)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    resultWaitingEvent.WaitOne();
                }

                if (thrownException != null)
                {
                    throw new AggregateException(thrownException);
                }

                return result ?? throw new InvalidOperationException();
            }
        }

        /// <summary>
        /// apply method to the result of this task.
        /// </summary>
        /// <param name="continueMethod">method to be applied to the result of current task</param>
        /// <typeparam name="TNewResult">result type of <see cref="continueMethod"/></typeparam>
        /// <returns>result of method <see cref="continueMethod"/> with current task result in argument</returns>
        /// <exception cref="OperationCanceledException">shutdown requested.</exception>
        public IMyTask<TNewResult> ContinueWith<TNewResult>(Func<TResult, TNewResult> continueMethod)
        {
            ArgumentNullException.ThrowIfNull(continueMethod);
            
            threadPool.shutdownEvent.WaitOne();

            lock (continuationBlockObject)
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (IsCompleted)
                {
                    return threadPool.Submit(() => continueMethod(Result));
                }
                
                var newTask = new MyTask<TNewResult>(() => continueMethod(Result), cancellationToken, threadPool);
                continuationTasks.Add(() => newTask.Evaluate());

                return newTask;
            }
        }

        /// <summary>
        /// method to compute result of <see cref="method"/>
        /// </summary>
        /// <exception cref="OperationCanceledException">shutdown requested</exception>
        /// <exception cref="InvalidOperationException"><see cref="method"/> returned null</exception>
        public void Evaluate()
        {
            if (IsCompleted || cancellationToken.IsCancellationRequested)
            {
                throw new OperationCanceledException();
            }

            try
            {
                result = method.Invoke() ?? throw new InvalidOperationException();
            }
            catch (Exception e)
            {
                thrownException = e;
            }

            IsCompleted = true;

            lock (continuationBlockObject)
            {
                continuationTasks.ForEach(task => threadPool.Submit(task));
            }

            resultWaitingEvent.Set();
        }
    }
}