using MyNUnit;
using MyNUnit.Tests;
using MyNUnit.Tests.TestsResult;
using MyNUnitWeb.Models;
using ClassTestResult = MyNUnit.Tests.TestsResult.ClassTestResult;

namespace MyNUnitWeb.Domain;

public static class MappingExtensions
{
    public static FileTestResult ToFileTestResult(this AssemblyTestResult result)
        => new()
        {
            ClassTestResults = result.ClassTestResults.Select(ToWebClassTestResult).ToList(),
            Name = result.AssemblyName
        };

    public static Models.ClassTestResult ToWebClassTestResult(this ClassTestResult result)
        => new()
        {
            MethodTestResults = result.TestResults.Select(ToMethodTestResult).ToList(),
            Name = result.ClassName
        };

    public static MethodTestResult ToMethodTestResult(this TestResult result)
        => new()
        {
            Name = result.TestName,
            TestDuration = result.TestDuration,
            IsPassed = result is PassedTestResult,
            IsFailed = result is FailedTestResult,
            IsIgnored = result is IgnoredTestResult,
            FailReasons = result is FailedTestResult failTestResult ? Utils.GetFailReason(failTestResult) : null,
            IgnoredReason = result is IgnoredTestResult ignoreTestResult ? ignoreTestResult.IgnoredReason : null,
        };
}