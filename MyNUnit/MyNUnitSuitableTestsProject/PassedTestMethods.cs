using MyNUnit.Assert;
using MyNUnit.Attributes;
using MyNUnit.Tests.TestResults;

namespace ProjectForTest;

public class PassedTestMethods
{
    public static int beforeAndAfterClassInvokesAmount = 0;

    public static int beforeAndAfterInvokesAmount = 0;
    
    [BeforeClass]
    public static void BeforeClass()
    {
        ++beforeAndAfterClassInvokesAmount;
    }

    [AfterClass]
    public static void AfterClass()
    {
        ++beforeAndAfterClassInvokesAmount;
    }

    [Before]
    public void Before()
    {
        ++beforeAndAfterInvokesAmount;
    }
    
    [After]
    public void After()
    {
        ++beforeAndAfterInvokesAmount;
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