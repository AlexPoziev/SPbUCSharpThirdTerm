using MyNUnit.Attributes;

namespace MyNUnitInvalidTestsProject;

public class InvalidTests
{
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