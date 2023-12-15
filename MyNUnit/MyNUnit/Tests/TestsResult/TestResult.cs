namespace MyNUnit.Tests.TestsResult;

public record TestResult(
    string TestName,
    long TestDuration,
    string? AdditionalInfo = default
);