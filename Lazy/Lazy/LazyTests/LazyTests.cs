using Lazy;

namespace LazyTests;

public class LazyTests
{
    private static int invokeCounter = 0;
    
    [Test]
    public void MultiThreadLazyGetWithIncrementShouldReturnSameResult()
    {
        var mre = new ManualResetEvent(false);
        
        const int resultSize = 10;
        var value = 0;
        const int expectedResult = 1;
        
        var lazy = new MultiThreadLazy<int>(() => Interlocked.Increment(ref value));
        
        var result = new int[resultSize];
        var threads = new Thread[resultSize];

        for (var i = 0; i < resultSize; ++i)
        {
            var locali = i;
            threads[i] = new Thread(() =>
            {
                mre.WaitOne();
                result[locali] = lazy.Get();
            });
        }

        foreach (var thread in threads)
        {
            thread.Start();
        }
        
        Thread.Sleep(300);
        mre.Set();

        foreach (var thread in threads)
        {
            thread.Join();
        }

        foreach (var element in result)
        {
            Assert.That(element, Is.EqualTo(expectedResult));
        }
    }

    [TestCaseSource(nameof(TestOneThreadMethodInvocation))]
    public void SingleThreadedLazyShouldReturnExpectedResult(ILazy<int> lazy)
    {
        const int expectedResult = 1;
        
        var firstResult = lazy.Get();
        var secondResult = lazy.Get();
        
        Assert.That(firstResult.Equals(secondResult) && firstResult.Equals(expectedResult));
        
        invokeCounter = 0;
    }
    
    [TestCaseSource(nameof(TestLaziesException))]
    public void ResultAfterExceptionShouldBeException(ILazy<int> lazy)
    {
        const int expectedResult = 1;

        Assert.Throws<AggregateException>(() => lazy.Get());
        Assert.Throws<AggregateException>(() => lazy.Get());
     
        Assert.That(invokeCounter, Is.EqualTo(expectedResult));

        invokeCounter = 0;
    }

    private static IEnumerable<ILazy<int>> TestLaziesException()
    {
        var method = () =>
        {
            var exceptionArray = new int[1];
            Interlocked.Increment(ref invokeCounter);
            return exceptionArray[invokeCounter];
        };

        yield return new Lazy.Lazy<int>(method);
        yield return new MultiThreadLazy<int>(method);
    }

    private static IEnumerable<ILazy<int>> TestOneThreadMethodInvocation()
    {
        yield return new Lazy.Lazy<int>(() => Interlocked.Increment(ref invokeCounter));
        yield return new MultiThreadLazy<int>(() => Interlocked.Increment(ref invokeCounter));
    }
}