using Microsoft.EntityFrameworkCore;
using MyNUnitWeb.Models;

namespace MyNUnitWeb.Repositories;

public class FileTestsRepository(FileTestContext fileTestContext) : IFileTestsRepository
{
    public async Task<FileTestResult[]> GetAllAsync()
    {
        return await fileTestContext.Set<FileTestResult>()
            .AsNoTracking()
            .Include(f => f.ClassTestResults)
            .ThenInclude(c => c.MethodTestResults)
            .ToArrayAsync();
    }

    public async Task<FileTestResult> GetAsync(long id)
    {
        return await fileTestContext.Set<FileTestResult>()
            .AsNoTracking()
            .Where(f => f.Id == id)
            .Include(f => f.ClassTestResults)
            .ThenInclude(c => c.MethodTestResults)
            .FirstOrDefaultAsync() ?? new FileTestResult();
    }

    public async Task<long> AddAsync(FileTestResult result)
    {
        await fileTestContext.AddAsync(result);
        await fileTestContext.SaveChangesAsync();

        return result.Id;
    }

    public async Task<long[]> AddAllAsync(FileTestResult[] results)
    {
        await fileTestContext.Set<FileTestResult>().AddRangeAsync(results);
        await fileTestContext.SaveChangesAsync();

        return results.Select(result => result.Id).ToArray();
    }
}