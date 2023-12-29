using System.Reflection;
using MyNUnitWeb.Models;

namespace MyNUnitWeb.Services;

public interface ITestsService
{
    public Task<FileTestResult> TestFileAsync(Assembly assembly);
    
    public Task<FileTestResult[]> TestFilesAsync(Assembly[] assemblies);

    public Task<FileTestResult[]> GetAllTestsAsync();
}