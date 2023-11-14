using System.ComponentModel.Design.Serialization;
using System.Diagnostics;
using System.Reflection;
using MyNUnit.Assert;
using MyNUnit.Attributes;
using MyNUnit.TestResults;

namespace MyNUnit;

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

        InvokeBeforeMethods();
        
        var stopWatch = new Stopwatch();
        stopWatch.Start();

        try
        {
            testMethodInfo.Invoke(classInstance, Array.Empty<object>());
            
            var result = new PassedTestResult(testName, stopWatch.Elapsed);
            
            InvokeAfterMethods();

            return result;
        }
        catch (FailedAssertion e)
        {
            stopWatch.Stop();
            
            InvokeAfterMethods();

            return new FailedTestResult(testName, stopWatch.Elapsed, true, AdditionalInfo: e.Message);
        }
        catch (Exception e)
        {
            stopWatch.Stop();

            var exceptionType = e.GetType();

            TestResult result = exceptionType == expectedExceptionType
                ? new PassedTestResult(testName, stopWatch.Elapsed)
                : new FailedTestResult(testName, stopWatch.Elapsed, false, expectedExceptionType, exceptionType);

            InvokeAfterMethods();

            return result;
        }
    }
    
    private void InvokeBeforeMethods() 
        => beforeTestsMethodInfo.ForEach(methodInfo => methodInfo.Invoke(classInstance, null));
    
    private void InvokeAfterMethods() 
        => afterTestsMethodInfo.ForEach(methodInfo => methodInfo.Invoke(classInstance, null));
}