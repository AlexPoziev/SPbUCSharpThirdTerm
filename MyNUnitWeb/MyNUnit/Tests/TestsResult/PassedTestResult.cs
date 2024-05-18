namespace MyNUnit.Tests.TestsResult;

public record PassedTestResult(
    string TestName,
    long TestDuration,
    string? AdditionalInfo = default
) : TestResult(TestName, TestDuration, AdditionalInfo);