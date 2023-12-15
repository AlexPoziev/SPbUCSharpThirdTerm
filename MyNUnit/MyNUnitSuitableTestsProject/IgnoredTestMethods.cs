using MyNUnit.Assert;
using MyNUnit.Attributes;

namespace ProjectForTest;

public class IgnoredTestMethods
{
    [Test("The reason is not important")]
    public void TestShouldBeIgnored()
    {
        
    }
    
    [Test("bla-bla-bla", typeof(InvalidDataException))]
    public void TestWithExceptionShouldBeIgnored()
    {
        throw new InvalidCastException();
    }

    [Test("bla-bla-bla")]
    public void TestWithAssertionShouldBeIgnored()
    {
        const int temp = 2;

        Assert.That(2 != temp);
    }
}