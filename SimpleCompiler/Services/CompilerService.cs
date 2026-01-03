using System.Diagnostics;
using Microsoft.Extensions.Options;
using SimpleCompiler.Interfaces;
using SimpleCompiler.Models;
using SimpleCompiler.Settings;

namespace SimpleCompiler.Services;

public class CompilerService(IOptions<CompilerSettings> compilerSettings) : ICompilerService
{
    private readonly CompilerSettings _settings = compilerSettings.Value;

    public async Task<CompilerResponse> RunCode(string code, IFormFile inputFile)
    {
        var path = await CreateFiles(code, inputFile);

        var compilationResult = await Compile(path);

        if (!compilationResult.Success)
        {
            return compilationResult;
        }

        var executionResult = await Execute(path);

        DeleteFiles(path);

        return executionResult;
    }

    public async Task<CompilerResponse> RunCode(string code)
    {
        var path = await CreateFiles(code);

        var compilationResult = await Compile(path);

        if (!compilationResult.Success)
        {
            return compilationResult;
        }

        var executionResult = await Execute(path);

        DeleteFiles(path);

        return executionResult;
    }

    private static async Task<CompilerResponse> Compile(string path)
    {
        await Run("dotnet", "new console -n app --force", path);

        File.Copy(Path.Combine(path, "UserCode.cs"), Path.Combine(path, "app", "Program.cs"), overwrite: true);

        var inputFilePath = Path.Combine(path, "Input.txt");
        if (File.Exists(inputFilePath))
        {
            File.Copy(inputFilePath, Path.Combine(path, "app", "Input.txt"), overwrite: true);
        }

        return await Run("dotnet", "publish -c Release -o out", Path.Combine(path, "app"));
    }

    private static async Task<CompilerResponse> Execute(string path)
    {
        var app = Path.Combine(path, "app", "out", "app.dll");

        using var process = new Process();
        process.StartInfo = new ProcessStartInfo
        {
            FileName = "dotnet",
            Arguments = app,
            WorkingDirectory = path,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        process.Start();

        var output = await process.StandardOutput.ReadToEndAsync();
        var error = await process.StandardError.ReadToEndAsync();
        process.WaitForExit(10000);

        return error != "" ? new CompilerResponse(false, error) : new CompilerResponse(true, output);
    }

    private static async Task<CompilerResponse> Run(string fileName, string args, string workingDir)
    {
        using var process = Process.Start(new ProcessStartInfo
        {
            FileName = fileName,
            Arguments = args,
            WorkingDirectory = workingDir,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        });

        var output = await process!.StandardOutput.ReadToEndAsync();
        var error = await process.StandardError.ReadToEndAsync();
        await process.WaitForExitAsync();

        if (output.Contains("error"))
        {
            return new CompilerResponse(false, output);
        }

        return error != "" ? new CompilerResponse(false, error) : new CompilerResponse(true, string.Empty);
    }

    private async Task<string> CreateFiles(string code, IFormFile? inputFile = null)
    {
        var id = Guid.NewGuid().ToString();
        var path = _settings.Path;

        var destDirPath = Path.Combine(path, id);
        Directory.CreateDirectory(destDirPath);

        var destCodePath = Path.Combine(destDirPath, "UserCode.cs");
        await File.Create(destCodePath).DisposeAsync();

        await File.WriteAllTextAsync(destCodePath, code);

        if (inputFile == null) return destDirPath;
        
        var destInputPath = Path.Combine(destDirPath, "Input.txt");
        await using var stream = new FileStream(destInputPath, FileMode.Create);
        await inputFile.CopyToAsync(stream);

        return destDirPath;
    }

    private static void DeleteFiles(string path)
    {
        if (Directory.Exists(path))
        {
            Directory.Delete(path, true);
        }
    }
}