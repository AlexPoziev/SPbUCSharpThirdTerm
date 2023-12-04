using System.Reflection;
using MyNUnit.Tests;
using MyNUnit.Tests.TestsResult;
using MyNUnitWeb.Domain;
using MyNUnitWeb.Models;
using MyNUnitWeb.Repositories;

namespace MyNUnitWeb.Services;

public class TestsService : ITestsService
{
    private readonly IFileTestsRepository _testRepository;
    private readonly FileTestContext _dbContext;

    public TestsService(IFileTestsRepository testRepository, FileTestContext dbContext)
    {
        _testRepository = testRepository;
        _dbContext = dbContext;
    }

    public async Task<FileTestResult> TestFileAsync(Assembly assembly)
    {
        var testResult = AssemblyTest.RunTestByAssembly(assembly);
        var fileTestResult = testResult.ToFileTestResult(); 
        
        var id = await _testRepository.AddAsync(fileTestResult);
        fileTestResult.Id = id;

        return fileTestResult;
    }

    public async Task<FileTestResult[]> TestFilesAsync(Assembly[] assemblies)
    {
        var testResults = assemblies.Select(AssemblyTest.RunTestByAssembly);
        var fileTestResults = testResults.Select(result => result.ToFileTestResult()).ToArray();

        var ids = await _testRepository.AddAllAsync(fileTestResults);
        for (var i = 0; i < ids.Length; i++)
        {
            fileTestResults[i].Id = ids[i];
        }

        return fileTestResults;
    }

    public async Task<FileTestResult[]> GetAllTestsAsync()
    {
        var allTests = await _testRepository.GetAllAsync();

        return allTests;
    }
}