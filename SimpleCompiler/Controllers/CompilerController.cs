using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SimpleCompiler.Interfaces;
using SimpleCompiler.Models;
using SimpleCompiler.Services;
using SimpleCompiler.Settings;

namespace SimpleCompiler.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CompilerController(ICompilerService compilerService, IOptions<CompilerSettings> compilerSettings) : ControllerBase
{
    
    [HttpPost("run-code")]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<CompilerResponse>> RunCode([FromForm] string code)
    {
        var bytes = Convert.FromBase64String(code);
        var result = await compilerService.RunCode(Encoding.UTF8.GetString(bytes));
        return Ok(result);
    }
    
    [HttpPost("run-code-with-input")]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<CompilerResponse>> RunCode(IFormFile inputFile, [FromForm] string code)
    {
        var maxFileSize = compilerSettings.Value.MaxFileSize * 1024 * 1024;

        if (inputFile.Length > maxFileSize)
        {
            return BadRequest(new CompilerResponse(false, "File too large. Max 1 MB"));
        }

        if (!inputFile.FileName.EndsWith(".txt", StringComparison.OrdinalIgnoreCase))
            return BadRequest(new CompilerResponse(false, "Only .txt files are allowed"));
        
        var bytes = Convert.FromBase64String(code);
        var result = await compilerService.RunCode(Encoding.UTF8.GetString(bytes), inputFile);

        return Ok(result);
    }
    
}