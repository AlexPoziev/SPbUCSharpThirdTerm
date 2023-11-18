using System.Collections.Concurrent;
using MyNUnit.Tests;
using MyNUnit.Tests.TestsResult;

namespace MyNUnit;

public static class TestRunner
{
    public static List<AssemblyTestResult> RunTests(string path)
    {
        var result = new ConcurrentBag<AssemblyTestResult>();

        var assemblies = Directory.EnumerateFiles(path, "*.dll")
            .Select(a => new AssemblyTest(a));
        
        Parallel.ForEach(assemblies, assembly => result.Add(assembly.Run()));

        return result.ToList();
    }
}