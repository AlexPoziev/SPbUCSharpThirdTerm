namespace MyNUnit.Tests.TestsResult;

public record IgnoredTestResult(
    string TestName,
    string IgnoredReason,
    string? AdditionalInfo = null
) : TestResult(TestName, 0, AdditionalInfo);