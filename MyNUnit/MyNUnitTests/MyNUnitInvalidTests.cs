using System.Reflection;
using MyNUnit.Tests;
using MyNUnit.Tests.TestResults;

namespace MyNUnitTests;

public class MyNUnitInvalidTests
{
    private AssemblyTestResult testResult = null!;

    private const string path = "./TestDlls/MyNUnitInvalidTestsProject.dll";

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
        const int expectedNumber = 0;

        var successTestsType = assembly.GetTypes().First(t => t.Name == "InvalidTests");
        var actualResult = successTestsType.GetField("beforeAndAfterInvokesAmount")!.GetValue(null);
        
        Assert.That(actualResult, Is.EqualTo(expectedNumber));
    }
    
    [Test]
    public void BeforeAndAfterClassInvokesNumberShouldBeExpected()
    {
        const int expectedNumber = 0;

        var successTestsType = assembly.GetTypes().First(t => t.Name == "InvalidTests");
        var actualResult = successTestsType.GetField("beforeAndAfterClassInvokesAmount")!.GetValue(null);
        
        Assert.That(actualResult, Is.EqualTo(expectedNumber));
    }
}