using Microsoft.EntityFrameworkCore;

namespace MyNUnitWeb.Models;

public class FileTestContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<FileTestResult> FileTestResults { get; set; } = null!;
}