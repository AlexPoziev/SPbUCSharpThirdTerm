using System.Collections.Concurrent;

namespace MyThreadPool;

public class MyThreadPool
{
    private Semaphore threadRunController;
    private CancellationTokenSource cancellationTokenSource;
    private ConcurrentQueue<Action> tasksQueue;
    private MyThread[] threads;
    private ManualResetEvent shutdownEvent;
    private object lockObject;

    public MyThreadPool(int threadsNumber)
    {
        if (threadsNumber <= 0)
        {
            throw new ArgumentException( "threads number must to be more than 0", nameof(threadsNumber));
        }

        shutdownEvent = new(true);
        threadRunController = new(0, int.MaxValue);
        cancellationTokenSource = new CancellationTokenSource();
        tasksQueue = new();
        lockObject = new();
        threads = Enumerable.Range(0, threadsNumber).Select(i => new MyThread(this, i, cancellationTokenSource.Token)).ToArray();
    }

    public IMyTask<TResult> Submit<TResult>(Func<TResult> function)
    {
        ArgumentNullException.ThrowIfNull(function);

        shutdownEvent.WaitOne();
        
        lock (lockObject)
        {
            if (cancellationTokenSource.IsCancellationRequested)
            {
                throw new InvalidOperationException();
            }
            
            var newTask = new MyTask<TResult>(function, cancellationTokenSource.Token);
            tasksQueue.Enqueue(() => newTask.Compute());
            var check = threadRunController.Release();

            return newTask;
        }
    }

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
    
    private class MyThread
    {
        private int number;
        public bool IsWorking { get; private set; }
        
        private Thread thread;
        private MyThreadPool threadPool;
        private CancellationToken cancellationToken;

        public MyThread(MyThreadPool threadPool, int number, CancellationToken cancellationToken)
        {
            this.cancellationToken = cancellationToken;
            this.number = number;
            this.threadPool = threadPool;
            thread = CreateThread();
            thread.Start();
        }

        public void Join()
        {
            thread.Join();
        }
        
        private Thread CreateThread()
        {
            return new Thread(() =>
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    threadPool.threadRunController.WaitOne();

                    if (threadPool.tasksQueue.TryDequeue(out var task) && !cancellationToken.IsCancellationRequested)
                    {
                        IsWorking = true;
                        task.Invoke();
                        IsWorking = false;
                    }
                }
            });
        }
    }

    private class MyTask<TResult> : IMyTask<TResult>
    {
        public bool IsCompleted { get; private set; }

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

                return result;
            }
        }

        private readonly Func<TResult> function;
        private Exception? thrownException;
        private TResult result;
        private readonly ManualResetEvent resultWaitingEvent;
        private readonly CancellationToken cancellationToken;
        private List<Action> continuationTasks;
        
        public MyTask(Func<TResult> function, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(function);

            this.cancellationToken = cancellationToken;
            this.function = function;
            resultWaitingEvent = new(false);
            continuationTasks = new();
        }
        
        public TNewResult ContinueWith<TNewResult>(Func<TResult, TNewResult> continueMethod)
        {
            
        }
        
        public void Compute()
        {
            if (IsCompleted || cancellationToken.IsCancellationRequested)
            {
                throw new InvalidOperationException();
            }
            
            try
            {
                result = function.Invoke();
            }
            catch (Exception e)
            {
                thrownException = e;
            }

            IsCompleted = true;
            resultWaitingEvent.Set();
        }
    }
}