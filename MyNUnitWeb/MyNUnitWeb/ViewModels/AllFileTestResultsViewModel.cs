using MyNUnitWeb.Models;

namespace MyNUnitWeb.ViewModels;

public class AllFileTestResultsViewModel
{
    public List<FileTestResultViewModel> FileTestResults { get; set; } = null!;
}