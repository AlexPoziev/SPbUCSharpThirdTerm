using System.ComponentModel.DataAnnotations;

namespace MyNUnitWeb.Models;

public class FileTestResult
{
    [Key]
    public long Id { get; set; }

    public string Name { get; set; } = null!;

    public List<ClassTestResult> ClassTestResults { get; set; } = null!;
}