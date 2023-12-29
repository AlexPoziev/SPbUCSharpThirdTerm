namespace MyNUnitWeb.ViewModels;

public class ResultViewModel
{
    public long Id { get; set; }

    public string Name { get; set; } = null!;
    
    public bool IsFailed { get; set; }
    
    public bool IsPassed { get; set; }
    
    public bool IsIgnored { get; set; }
    
    public long TestDuration { get; set; }
}