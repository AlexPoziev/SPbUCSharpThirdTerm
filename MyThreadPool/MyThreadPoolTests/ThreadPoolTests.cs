namespace MyThreadPoolTests;

using MyThreadPool;

public class MyTaskTest
{
    private readonly int threadPoolSize = 5;
    private MyThreadPool threadPool = null!;
    private ManualResetEvent mre = new(false);

    [SetUp]
    public void Initialization()
    {
        threadPool = new(threadPoolSize);
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
                Thread.Sleep(500);

                return 1;
            });
        }
        
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
        var expectedResult = 400;   
        var threadsCount = 10;
        var threads = new Thread[threadsCount];
        var test = new IMyTask<int>[threadPoolSize * threadsCount];

        for (var i = 0; i < threadsCount; ++i)
        {
            var locali = i;
            threads[locali] = new Thread(() =>
                {
                    mre.WaitOne();

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

        mre.Set();

        foreach (var thread in threads)
        {
            thread.Join();
        }

        var result = test.Sum(x => x.Result);
        
        Assert.That(result, Is.EqualTo(expectedResult));

        mre.Reset();
    }
}