using MyNUnitWeb.Models;

namespace MyNUnitWeb.Repositories;

public class FileTestsRepository : IFileTestsRepository
{
    public Task<FileTestResult[]> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public Task<long> GetAsync(long id)
    {
        throw new NotImplementedException();
    }

    public Task<long> AddAsync(FileTestResult result)
    {
        throw new NotImplementedException();
    }
}