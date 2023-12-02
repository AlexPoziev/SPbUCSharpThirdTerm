using System.ComponentModel.DataAnnotations;

namespace MyNUnitWeb.Models;

public class ClassTestResult
{
    [Key]
    public long Id { get; set; }

    public string Name { get; set; } = null!;

    public List<MethodTestResult> MethodTestResults { get; set; } = null!;
}