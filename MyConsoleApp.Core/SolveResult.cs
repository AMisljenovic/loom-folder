namespace MyConsoleApp.Core;

/// <summary>
/// Represents the outcome of evaluating an arithmetic expression without throwing an exception.
/// </summary>
/// <param name="Success">Indicates whether evaluation succeeded.</param>
/// <param name="Value">The computed value when evaluation succeeds; otherwise 0.</param>
/// <param name="Error">The error message when evaluation fails; otherwise null.</param>
public sealed record SolveResult(bool Success, double Value, string? Error)
{
    public static SolveResult FromValue(double value) => new(true, value, null);

    public static SolveResult FromError(string error) => new(false, default, error);
}
