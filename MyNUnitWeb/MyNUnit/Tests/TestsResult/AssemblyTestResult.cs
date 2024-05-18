namespace MyNUnit.Tests.TestsResult;

public record AssemblyTestResult(
    List<ClassTestResult> ClassTestResults,
    string AssemblyName,
    long TestDuration
);