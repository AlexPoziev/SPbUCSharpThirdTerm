namespace MyNUnitWeb.ViewModels;

public class MethodTestResultViewModel : ResultViewModel
{
    public string? FailReasons { get; set; }
    
    public string? IgnoredReason { get; set; }
}