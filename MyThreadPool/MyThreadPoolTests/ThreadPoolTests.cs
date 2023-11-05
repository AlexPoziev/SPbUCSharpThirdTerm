namespace MyThreadPoolTests;

using MyThreadPool;

public class MyTaskTest
{
    private readonly int threadPoolSize = 5;
    private MyThreadPool threadPool = null!;
    private ManualResetEvent manualResetEvent = new(false);

    [SetUp]
    public void Initialization()
    {
        threadPool = new(threadPoolSize);
        manualResetEvent = new(false);
    }
    
    [TearDown]
    public void Cleanup()
    {
        threadPool.Shutdown();
    }
    
    [Test]
    public void SingleTaskSubmitShouldHaveExpectedResult()
    {
        const int expectedResult = 92;
        
        var task = threadPool.Submit(() => 
        { 
            Thread.Sleep(100);
            return 46 * 2;
        });
        
        Assert.That(task.Result, Is.EqualTo(expectedResult));
    }

    [Test]
    public void ThreadPoolShouldUseAllThreads()
    {
        for (var i = 0; i < threadPoolSize * 5; ++i)
        {
            threadPool.Submit(() =>
            {
                Thread.Sleep(100);

                return 1;
            });
        }
        
        Thread.Sleep(200);
        
        Assert.That(threadPool.WorkingThreadsNumber, Is.EqualTo(threadPoolSize));
    }
    
    [Test]
    public void MultipleContinueWithShouldHaveExpectedResult()
    {
        var expectedResult = 442;

        var myTask = threadPool.Submit(() => 2 * 2)
            .ContinueWith(x => x.ToString())
            .ContinueWith(x => x + "42")
            .ContinueWith(int.Parse);
        
        Assert.That(myTask.Result, Is.EqualTo(expectedResult));
    }

    [Test]
    public void ResultTaskWithExceptionShouldThrowsAggregateException()
    {
        var myTask = threadPool.Submit(() =>
        {
            var exceptionArray = new int[1];
            return exceptionArray[1];
        });
        
        Assert.Throws<AggregateException>(() =>
        {
            var test = myTask.Result;
        });
    }

    [Test]
    public void ContinueWithAndSubmitAfterShutdownShouldThrowsOperationCanceledException()
    {
        var myTask = threadPool.Submit(() => 2 * 2);
        
        Thread.Sleep(100);
        threadPool.Shutdown();
        
        Assert.Throws<OperationCanceledException>(() => threadPool.Submit(() => 2 * 2));
        Assert.Throws<OperationCanceledException>(() => myTask.ContinueWith(x => x.ToString()));
    }
    
    [Test]
    public void SubmitAndContinueWithFromMultipleThreadsShouldPerformExpectedResult()
    {
        const int expectedResult = 400;   
        const int threadsCount = 10;
        var threads = new Thread[threadsCount];
        var test = new IMyTask<int>[threadPoolSize * threadsCount];

        for (var i = 0; i < threadsCount; ++i)
        {
            var locali = i;
            threads[locali] = new Thread(() =>
                {
                    manualResetEvent.WaitOne();

                    for (var j = threadPoolSize * locali; j < threadPoolSize * (locali + 1); ++j)
                    {
                        test[j] = threadPool.Submit(() => 2 + 2).ContinueWith(r => r * 2);
                    }
                }
            );
        }

        foreach (var thread in threads)
        {
            thread.Start();
        }
        
        Thread.Sleep(100);

        manualResetEvent.Set();

        foreach (var thread in threads)
        {
            thread.Join();
        }

        var result = test.Sum(x => x.Result);
        
        Assert.That(result, Is.EqualTo(expectedResult));
    }
    
    [Test]
    public async Task ConcurrentSubmitAndShutdownShouldPerformExpectedResult()
    {
        const int expectedResult = 3628800;
        const int factorialNumber = 10;
        var actualResult = 0;
        var submitTask = Task.Run(() =>
        {
            manualResetEvent.WaitOne();
            
            return threadPool.Submit(() => Enumerable.Range(1, factorialNumber).Aggregate(1, (a, b) => a * b));
        });
        var shutdownTak = Task.Run(() =>
        {
            manualResetEvent.WaitOne();
            
            threadPool.Shutdown();
        });

        try
        {
            manualResetEvent.Set(); 
            actualResult = (await submitTask).Result;
        }
        catch (OperationCanceledException)
        {
            Assert.Pass();
        }

        Assert.That(actualResult, Is.EqualTo(expectedResult));
    }

    [Test]
    public void IsCompletedShouldPerformExpectedBehaviour()
    {
        var task = threadPool.Submit(() =>
        {
            manualResetEvent.WaitOne();

            return 0;
        });
        
        Assert.That(task.IsCompleted, Is.False);
        
        manualResetEvent.Set();
        
        Thread.Sleep(100);
        
        Assert.That(task.IsCompleted, Is.True);
    }
}