using Microsoft.EntityFrameworkCore;

namespace MyNUnitWeb.Models;

public class FileTestContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<FileTestResult> FileTestResults { get; set; } = null!;
    
    public DbSet<ClassTestResult> ClassTestResults { get; set; } = null!;
    
    public DbSet<MethodTestResult> MethodTestResults { get; set; } = null!;
}