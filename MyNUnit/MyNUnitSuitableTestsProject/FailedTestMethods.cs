using MyNUnit.Assert;
using MyNUnit.Attributes;

namespace ProjectForTest;

public class FailedTestMethods
{
    [Test(typeof(InvalidDataException))]
    public void TestWithExceptionShouldBeFailed()
    {
        throw new InvalidOperationException();
    }
    
    [Test]
    public void TestWithAssertionShouldBeFailed()
    {
        Assert.That(2 == 1);
    }
}