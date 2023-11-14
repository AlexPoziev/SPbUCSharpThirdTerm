using FluentValidation.Results;
using MyNUnit.TestResults;

namespace MyNUnit;

public record ClassTestResult(
    List<TestResult> TestResults,
    List<ValidationFailure> MethodsValidationErrors,
    TimeSpan? Duration = null
    );
