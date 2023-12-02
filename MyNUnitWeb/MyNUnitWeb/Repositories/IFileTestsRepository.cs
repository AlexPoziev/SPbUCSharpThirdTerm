using MyNUnitWeb.Models;

namespace MyNUnitWeb.Repositories;

public interface IFileTestsRepository
{
    public Task<FileTestResult[]> GetAllAsync();

    public Task<long> GetAsync(long id); 
    
    public Task<long> AddAsync(FileTestResult result);
}