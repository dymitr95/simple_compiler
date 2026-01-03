using SimpleCompiler.Models;

namespace SimpleCompiler.Interfaces;

public interface ICompilerService
{
    public Task<CompilerResponse> RunCode(string code, IFormFile inputFile);
    public Task<CompilerResponse> RunCode(string code);
}