namespace MyConsoleApp.Core.Errors;

/// <summary>
/// Represents an error that occurs while tokenizing, parsing, or evaluating a math expression.
/// </summary>
public sealed class MathSolverException : Exception
{
    public MathSolverException(string message, int? position = null, string? errorCode = null)
        : base(message)
    {
        Position = position;
        ErrorCode = errorCode;
    }

    public int? Position { get; }

    public string? ErrorCode { get; }
}
