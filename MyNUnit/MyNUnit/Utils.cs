using System.Reflection;
using MyNUnit.Tests.TestsResult;

namespace MyNUnit;

public static class Utils
{
    public static List<MethodInfo> GetMethodsWithAttributes(Type attributeType, IEnumerable<MethodInfo> methods)
        => methods.Where(method => Attribute.IsDefined(method, attributeType)).ToList();

    public static async Task PrintReportOfAssemblyTestAsync(AssemblyTestResult assemblyTestResult, TextWriter writer)
    {
        await writer.WriteLineAsync($"Assembly name: {assemblyTestResult.AssemblyName}");
        await writer.WriteLineAsync($"Duration: {assemblyTestResult.TestDuration}");

        foreach (var result in assemblyTestResult.ClassTestResults)
        {
            await PrintReportOfClassTestAsync(result, writer);
        }
        
        await writer.WriteLineAsync("================================================================");
    }
    
    public static async Task PrintReportOfClassTestAsync(ClassTestResult classTestResult, TextWriter writer)
    {

        await writer.WriteLineAsync($"\tClass name: {classTestResult.ClassName}");
        await writer.WriteLineAsync($"\tDuration: {classTestResult.TestDuration}");

        if (classTestResult.MethodsValidationErrors is not null)
        {
            await writer.WriteLineAsync($"\tTests result: Invalid methods.");
            await writer.WriteLineAsync($"!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
            classTestResult.MethodsValidationErrors.ForEach(method => Console.WriteLine($" {method.ErrorMessage}"));
            await writer.WriteLineAsync($"!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");

            return;
        }

        foreach (var result in classTestResult.TestResults)
        {
            await PrintReportOfTestAsync(result, writer);
        }
        
        await writer.WriteLineAsync("----------------------------------------------------------------");
    }
    
    public static async Task PrintReportOfTestAsync(TestResult result, TextWriter writer)
    {
        await writer.WriteLineAsync($"\t\tTest name: {result.TestName}");
        await writer.WriteLineAsync($"\t\tDuration: {result.TestDuration}");

        if (result.AdditionalInfo is not null)
        {
            await writer.WriteLineAsync($"\t\tAdditional information: {result.AdditionalInfo}");
        }

        switch (result)
        {
            case PassedTestResult:
                await writer.WriteLineAsync("\t\tTest result: Passed");
                
                break;
            
            case FailedTestResult failedResult:
                await writer.WriteLineAsync("\t\tTest result: Failed");

                if (failedResult.IsAssertionFailed)
                {
                    await writer.WriteLineAsync("\t\tReason: Assertion Failed");
                }
                else
                {
                    await writer.WriteLineAsync($"\t\tReason: was expected {failedResult.ExpectedException?.Name} exception but was {failedResult.ActualException?.Name}");
                }

                break;
            
            case IgnoredTestResult ignoredTestResult:
                await writer.WriteLineAsync("\t\tTest result: Ignored");
                await writer.WriteLineAsync($"\t\tReason: {ignoredTestResult.IgnoredReason}");
                
                break;
        }
        
        await writer.WriteLineAsync();
    }
}