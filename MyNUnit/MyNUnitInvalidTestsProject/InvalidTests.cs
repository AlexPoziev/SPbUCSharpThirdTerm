using MyNUnit.Attributes;

namespace MyNUnitInvalidTestsProject;

public class InvalidTests
{
    public static int beforeAndAfterClassInvokesAmount = 0;

    public static int beforeAndAfterInvokesAmount = 0;
    
    [BeforeClass]
    public static void BeforeClass()
    {
        beforeAndAfterClassInvokesAmount += 10;
    }

    [AfterClass]
    public static void AfterClass()
    {
        ++beforeAndAfterClassInvokesAmount;
    }

    [Before]
    public void Before()
    {
        beforeAndAfterInvokesAmount += 10;
    }
    
    [After]
    public void After()
    {
        ++beforeAndAfterInvokesAmount;
    }
    
    [BeforeClass]
    public void NotStaticBeforeClass()
    {
        
    }
    
    [AfterClass]
    public void NotStaticAfterClass()
    {
        
    }

    [Test]
    public int TestNotVoidReturnType()
    {
        return 1;
    }

    [Test]
    public void TestWithParameters(int test)
    {
        
    }
}