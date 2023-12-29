using System.Reflection;
using MyNUnit.Tests.TestsResult;

namespace MyNUnit;

public static class Utils
{
    public static List<MethodInfo> GetMethodsWithAttributes(Type attributeType, IEnumerable<MethodInfo> methods)
        => methods.Where(method => Attribute.IsDefined(method, attributeType)).ToList();
    
    public static string GetFailReason(FailedTestResult result)
        => result.IsAssertionFailed ? "Some Assertion Failed" : $"Expected exception was: {result.ExpectedException?.Name}, but actually was {result.ActualException?.Name}";
}