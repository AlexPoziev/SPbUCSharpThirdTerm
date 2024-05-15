using MyNUnit.Assert;
using MyNUnit.Attributes;

namespace ProjectForTest;

public class PassedTestMethods
{
    public static volatile int beforeAndAfterClassInvokesAmount;

    public static volatile int beforeAndAfterInvokesAmount;
    
    [BeforeClass]
    public static void BeforeClass()
    {
        Interlocked.Add(ref beforeAndAfterClassInvokesAmount, 10);
    }

    [AfterClass]
    public static void AfterClass()
    {
        Interlocked.Increment(ref beforeAndAfterClassInvokesAmount);
    }

    [Before]
    public void Before()
    {
        Interlocked.Add(ref beforeAndAfterInvokesAmount, 10);
    }
    
    [After]
    public void After()
    {
        Interlocked.Increment(ref beforeAndAfterInvokesAmount);
    }
    
    [Test(typeof(InvalidDataException))]
    public void TestWithExceptionShouldBePassed()
    {
        throw new InvalidDataException();
    }

    [Test]
    public void TestWithAssertionShouldBePassed()
    {
        const int temp = 2;
        
        Assert.That(2 == temp);
    }
}