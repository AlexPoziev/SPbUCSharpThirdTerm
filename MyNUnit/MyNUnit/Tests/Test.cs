using System.Diagnostics;
using System.Reflection;
using MyNUnit.Assert;
using MyNUnit.Attributes;
using MyNUnit.Tests.TestResults;

namespace MyNUnit.Tests;

public class Test
{
    private readonly Type? expectedExceptionType;

    private readonly string? ignoreReason;
    
    private readonly object classInstance;

    private readonly MethodInfo testMethodInfo;
    
    private readonly List<MethodInfo> beforeTestsMethodInfo;
    
    private readonly List<MethodInfo> afterTestsMethodInfo;
    
    private readonly string testName;

    private bool isStarted;

    public Test(object classInstance, MethodInfo testMethodInfo, List<MethodInfo> beforeTestsMethodInfo, List<MethodInfo> afterTestsMethodInfo)
    {
        this.classInstance = classInstance;
        this.testMethodInfo = testMethodInfo;
        this.beforeTestsMethodInfo = beforeTestsMethodInfo;
        this.afterTestsMethodInfo = afterTestsMethodInfo;
        
        testName = testMethodInfo.Name;
        
        var testAttribute = (TestAttribute)testMethodInfo.GetCustomAttribute(typeof(TestAttribute))!;
        ignoreReason = testAttribute.IgnoreMessage;
        expectedExceptionType = testAttribute.ExpectedException;
    }

    public TestResult Run()
    {
        isStarted = true;
        
        if (ignoreReason is not null)
        {
            return new IgnoredTestResult(testName, ignoreReason);
        }

        InvokeMethods(afterTestsMethodInfo);
        
        var stopWatch = new Stopwatch();
        stopWatch.Start();

        try
        {
            testMethodInfo.Invoke(classInstance, Array.Empty<object>());
            
            stopWatch.Stop();
            
            var result = new PassedTestResult(testName, stopWatch.ElapsedMilliseconds);
            
            InvokeMethods(afterTestsMethodInfo);

            return result;
        }
        catch (Exception e)
        {
            stopWatch.Stop();

            var exceptionType = e.InnerException?.GetType();

            TestResult result = exceptionType == typeof(FailedAssertionException)
                ? new FailedTestResult(testName, stopWatch.ElapsedMilliseconds, true, AdditionalInfo: e.Message)
                : exceptionType == expectedExceptionType
                    ? new PassedTestResult(testName, stopWatch.ElapsedMilliseconds)
                    : new FailedTestResult(testName, stopWatch.ElapsedMilliseconds, false, expectedExceptionType, exceptionType);

            InvokeMethods(afterTestsMethodInfo);

            return result;
        }
    }
    
    private void InvokeMethods(IEnumerable<MethodInfo> methods) 
        => Parallel.ForEach(methods, methodInfo => methodInfo.Invoke(classInstance, null));
}