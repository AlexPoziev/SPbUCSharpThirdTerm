using MyNUnit;
using MyNUnit.Tests.TestsResult;
using MyNUnitWeb.Models;
using MyNUnitWeb.ViewModels;
using ClassTestResult = MyNUnit.Tests.TestsResult.ClassTestResult;

namespace MyNUnitWeb.Domain;

public static class MappingExtensions
{
    public static MethodTestResultViewModel ToViewModel(this MethodTestResult result)
        => new()
        {
            Id = result.Id,
            Name = result.Name,
            TestDuration = result.TestDuration,
            IsPassed = result.IsPassed,
            IsFailed = result.IsFailed,
            IsIgnored = result.IsIgnored,
            FailReasons = result.FailReasons,
            IgnoredReason = result.IgnoredReason,
        };
    
    public static ClassTestResultViewModel ToViewModel(this Models.ClassTestResult result)
        => new()
        {
            Id = result.Id,
            Name = result.Name,
            TestDuration = result.MethodTestResults.Sum(t => t.TestDuration),
            IsPassed = result.MethodTestResults.All(t => t.IsPassed),
            IsFailed = result.MethodTestResults.Any(t => t.IsFailed),
            IsIgnored = result.MethodTestResults.All(t => t.IsIgnored),
            MethodTestResults = result.MethodTestResults.Select(ToViewModel).ToList(),
            ValidationErrors = result.ValidationErrors?.Split('/').ToList(),
        };

    public static FileTestResultViewModel ToViewModel(this FileTestResult result)
    {
        var classTestResultsViewModel = result.ClassTestResults.Select(ToViewModel).ToList();
        
    return new FileTestResultViewModel
    {
            Id = result.Id,
            Name = result.Name,
            TestDuration = classTestResultsViewModel.Sum(t => t.TestDuration),
            IsPassed = classTestResultsViewModel.All(t => t.IsPassed),
            IsFailed = classTestResultsViewModel.Any(t => t.IsFailed),
            IsIgnored = classTestResultsViewModel.All(t => t.IsIgnored),
            ClassTestResults = classTestResultsViewModel,
        };
    }
    
    public static AllFileTestResultsViewModel ToAllFileTestResultsViewModel(this IEnumerable<FileTestResult> results)
        => new()
        {
            FileTestResults = results.Select(ToViewModel).GroupBy(f => f.Name).SelectMany(g => g).ToList()
        };
    
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
            Name = result.ClassName,
            ValidationErrors = result.MethodsValidationErrors is null ? null : string.Join('/', result.MethodsValidationErrors.Select(v => v.ErrorMessage))
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