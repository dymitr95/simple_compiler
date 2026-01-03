using System.Diagnostics.CodeAnalysis;

namespace SimpleCompiler.Settings;

public class CompilerSettings
{
    public string Path { get; set; } = null!;
    public long MaxFileSize { get; set; }
}