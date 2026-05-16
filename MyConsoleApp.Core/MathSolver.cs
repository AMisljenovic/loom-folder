using MyConsoleApp.Core.Errors;

namespace MyConsoleApp.Core;

/// <summary>
/// Evaluates arithmetic expressions containing numbers, parentheses, constants, built-in functions, and the +, -, *, /, and ^ operators.
/// </summary>
public sealed class MathSolver
{
    private readonly ExpressionEvaluator _evaluator;

    public MathSolver()
        : this(new ExpressionEvaluator())
    {
    }

    internal MathSolver(ExpressionEvaluator evaluator)
    {
        _evaluator = evaluator;
    }

    /// <summary>
    /// Evaluates the specified expression and returns the computed value.
    /// </summary>
    /// <param name="input">The arithmetic expression to evaluate.</param>
    /// <returns>The evaluated numeric result.</returns>
    /// <exception cref="MathSolverException">Thrown when the input is empty or the expression is invalid.</exception>
    public double Solve(string input) => _evaluator.Evaluate(input);

    /// <summary>
    /// Evaluates the specified expression and returns either a value or an error without throwing.
    /// </summary>
    /// <param name="input">The arithmetic expression to evaluate.</param>
    /// <returns>A result object containing either the computed value or an error message.</returns>
    public SolveResult SolveSafely(string input)
    {
        try
        {
            return SolveResult.FromValue(_evaluator.Evaluate(input));
        }
        catch (MathSolverException ex)
        {
            return SolveResult.FromError(ex.Message);
        }
    }

    /// <summary>
    /// Attempts to evaluate the specified expression.
    /// </summary>
    /// <param name="input">The arithmetic expression to evaluate.</param>
    /// <param name="result">The computed value when evaluation succeeds; otherwise 0.</param>
    /// <param name="error">The validation or parsing error when evaluation fails; otherwise null.</param>
    /// <returns><c>true</c> when evaluation succeeds; otherwise <c>false</c>.</returns>
    [Obsolete("Prefer SolveSafely for non-throwing evaluation.")]
    public bool TrySolve(string input, out double result, out string? error)
    {
        var solveResult = SolveSafely(input);
        result = solveResult.Value;
        error = solveResult.Error;
        return solveResult.Success;
    }
}
