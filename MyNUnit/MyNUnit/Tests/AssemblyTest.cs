using System.Collections.Concurrent;
using System.Reflection;
using MyNUnit.Tests.TestResults;

namespace MyNUnit.Tests;

public class AssemblyTest
{
    private readonly string name;

    private readonly List<ClassTest> classTests;

    public AssemblyTest(string assemblyPath)
    {
        name = Path.GetFileName(assemblyPath);

        classTests = Assembly.LoadFrom(assemblyPath).ExportedTypes
            .Where(type => type.IsClass)
            .Select(type => new ClassTest(type.Name, type.GetTypeInfo(), name)).ToList();
    }

    public AssemblyTestResult Run()
    {
        var result = new ConcurrentBag<ClassTestResult>();
        
        Parallel.ForEach(classTests, test => result.Add(test.Run()));
        
        return new AssemblyTestResult(result.ToList(), name, result.Sum(r => r.TestDuration));
    }
}