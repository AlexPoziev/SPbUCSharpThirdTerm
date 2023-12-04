using System.Reflection;
using System.Runtime.Loader;
using Microsoft.AspNetCore.Mvc;
using MyNUnitWeb.Services;

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
    public async Task<IActionResult> UploadTests(IFormFileCollection files)
    {
        var tests = await _testsService
            .TestFilesAsync(files.Select(f => AssemblyLoadContext.Default.LoadFromStream(f.OpenReadStream())).ToArray());

        return Ok();
    }

    [HttpGet("testHistory")]
    public async Task<IActionResult> GetTestHistory()
    {
        throw new NotImplementedException();
    }
}