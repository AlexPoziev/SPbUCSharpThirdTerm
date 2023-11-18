namespace MyNUnit.Tests.TestsResult;

public record FailedTestResult(
    string TestName,
    long TestDuration,
    bool IsAssertionFailed,
    Type? ExpectedException = null,
    Type? ActualException = null,
    string? AdditionalInfo = null
) : TestResult(TestName, TestDuration, AdditionalInfo);