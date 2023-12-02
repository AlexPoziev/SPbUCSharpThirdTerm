using System.Reflection;
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

    public Task<FileTestResult> TestFileAsync(Assembly assembly)
    {
        throw new NotImplementedException();
    }

    public Task<FileTestResult[]> TestFilesAsync(Assembly[] assemblies)
    {
        throw new NotImplementedException();
    }

    public Task<FileTestResult[]> GetAllTestsAsync()
    {
        throw new NotImplementedException();
    }
}