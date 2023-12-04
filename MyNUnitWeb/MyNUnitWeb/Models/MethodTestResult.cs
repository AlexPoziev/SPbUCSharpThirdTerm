using System.ComponentModel.DataAnnotations;

namespace MyNUnitWeb.Models;

public class MethodTestResult
{
    [Key]
    public long Id { get; set; }

    public string Name { get; set; } = null!;

    public bool IsFailed { get; set; }
    
    public bool IsPassed { get; set; }
    
    public bool IsIgnored { get; set; }
    
    public string? FailReasons { get; set; }
    
    public string? IgnoredReason { get; set; }
    
    public long TestDuration { get; set; }
}