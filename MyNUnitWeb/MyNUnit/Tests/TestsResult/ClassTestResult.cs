using FluentValidation.Results;

namespace MyNUnit.Tests.TestsResult;

public record ClassTestResult(
    List<TestResult> TestResults,
    string ClassName,
    string AssemblyName,
    long TestDuration,
    List<ValidationFailure>? MethodsValidationErrors = null
);