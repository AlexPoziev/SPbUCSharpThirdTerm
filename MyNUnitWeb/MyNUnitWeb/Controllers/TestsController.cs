using System.Net;
using System.Reflection;
using System.Runtime.Loader;
using Microsoft.AspNetCore.Mvc;
using MyNUnitWeb.Domain;
using MyNUnitWeb.Models;
using MyNUnitWeb.Services;
using MyNUnitWeb.ViewModels;

namespace MyNUnitWeb.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TestsController : ControllerBase
{
    private readonly ITestsService _testsService;

    public TestsController(ITestsService testsService)
    {
        _testsService = testsService;
    }

    [HttpPost("test")]
    [ProducesResponseType(typeof(FileTestResultViewModel[]), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> UploadTests(IFormFileCollection files)
    {
        var tests = await _testsService
            .TestFilesAsync(files.Select(f => AssemblyLoadContext.Default.LoadFromStream(f.OpenReadStream())).ToArray());

        var testViewModels = tests.Select(t => t.ToViewModel()).ToArray();
        
        return Ok(testViewModels);
    }

    [HttpGet("testHistory")]
    [ProducesResponseType(typeof(AllFileTestResultsViewModel), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetTestHistory()
    {
        var allTests = await _testsService.GetAllTestsAsync();

        var result = allTests.ToAllFileTestResultsViewModel();
        
        return Ok(result);
    }
}