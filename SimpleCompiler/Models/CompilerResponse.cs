namespace SimpleCompiler.Models;

public class CompilerResponse (bool success, string message)
{
    public bool Success { get; set; } = success;
    public string Message { get; set; } = message;
}