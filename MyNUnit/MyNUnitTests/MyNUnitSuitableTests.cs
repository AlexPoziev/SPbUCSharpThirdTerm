using System.Reflection;
using MyNUnit.Tests;
using MyNUnit.Tests.TestsResult;

namespace MyNUnitTests;

public class MyNUnitSuitableTests
{
    private AssemblyTestResult testResult = null!;

    private const string path = "./TestDlls/MyNUnitSuitableTestsProject.dll";

    private Assembly assembly = null!;
    
    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        assembly = Assembly.LoadFrom(path);
        
        var assemblyTest = new AssemblyTest(assembly);

        testResult = assemblyTest.Run();
    }
    
    [Test]
    public void BeforeAndAfterInvokesNumberShouldBeExpected()
    {
        const int expectedNumber = 22;

        var successTestsType = assembly.GetTypes().First(t => t.Name == "PassedTestMethods");
        var actualResult = successTestsType.GetField("beforeAndAfterInvokesAmount")!.GetValue(null);
        
        Assert.That(actualResult, Is.EqualTo(expectedNumber));
    }
    
    [Test]
    public void BeforeAndAfterClassInvokesNumberShouldBeExpected()
    {
        const int expectedNumber = 11;

        var successTestsType = assembly.GetTypes().First(t => t.Name == "PassedTestMethods");
        var actualResult = successTestsType.GetField("beforeAndAfterClassInvokesAmount")!.GetValue(null);
        
        Assert.That(actualResult, Is.EqualTo(expectedNumber));
    }

    [TestCase("PassedTestMethods", typeof(PassedTestResult))]
    [TestCase("FailedTestMethods", typeof(FailedTestResult))]
    [TestCase("IgnoredTestMethods", typeof(IgnoredTestResult))]
    public void AllTestsInPassedTestMethodsShouldBePassed(string className, Type expectedType)
    {
        var actualResult = testResult.ClassTestResults
            .First(c => c.ClassName == className)
            .TestResults.All(t => t.GetType() == expectedType);

        Assert.That(actualResult, Is.True);
    }
}