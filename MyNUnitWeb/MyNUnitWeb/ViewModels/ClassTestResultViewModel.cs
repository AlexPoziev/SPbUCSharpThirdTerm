namespace MyNUnitWeb.ViewModels;

public class ClassTestResultViewModel : ResultViewModel
{
    public List<MethodTestResultViewModel> MethodTestResults { get; set; } = null!;
    
    public List<string>? ValidationErrors { get; set; } = null!;
}