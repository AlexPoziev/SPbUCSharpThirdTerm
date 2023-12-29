namespace MyNUnitWeb.ViewModels;

public class FileTestResultViewModel : ResultViewModel
{
    public List<ClassTestResultViewModel> ClassTestResults { get; set; } = null!;
}