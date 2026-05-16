using System.Globalization;
using MyConsoleApp.Core.Parsing;

namespace MyConsoleApp.Core.Errors;

internal static class MathSolverErrors
{
    public static MathSolverException InputCannotBeEmpty() => new("Input cannot be empty.", errorCode: "input.empty");

    public static MathSolverException UnexpectedToken(Token token) =>
        new($"Unexpected token '{token.Text}' at position {token.Position}.", token.Position, "parse.unexpected-token");

    public static MathSolverException MissingNumberValue(Token token) =>
        new($"Missing number value at position {token.Position}.", token.Position, "parse.missing-number-value");

    public static MathSolverException MissingClosingParenthesis(int openingPosition) =>
        new($"Missing closing parenthesis for '(' at position {openingPosition}.", openingPosition, "parse.missing-closing-parenthesis");

    public static MathSolverException ExpectedClosingParenthesisForFunction(string functionName) =>
        new($"Expected ')' after arguments for function '{functionName}'.", errorCode: "parse.missing-function-closing-parenthesis");

    public static MathSolverException DivisionByZero() => new("Division by zero.", errorCode: "eval.division-by-zero");

    public static MathSolverException InvalidCharacter(char character, int position) =>
        new($"Invalid character '{character}' at position {position}.", position, "token.invalid-character");

    public static MathSolverException InvalidNumber(string text, int position) =>
        new($"Invalid number '{text}' at position {position}.", position, "token.invalid-number");

    public static MathSolverException UnknownIdentifier(Token identifier) =>
        new($"Unknown identifier '{identifier.Text}' at position {identifier.Position}.", identifier.Position, "eval.unknown-identifier");

    public static MathSolverException UnknownFunction(Token identifier) =>
        new($"Unknown function '{identifier.Text}' at position {identifier.Position}.", identifier.Position, "eval.unknown-function");

    public static MathSolverException FunctionArityMismatch(string functionName, int expectedArity, int actualArity)
    {
        var expectedText = expectedArity.ToString(CultureInfo.InvariantCulture);
        var actualText = actualArity.ToString(CultureInfo.InvariantCulture);
        var suffix = expectedArity == 1 ? string.Empty : "s";
        return new($"Function '{functionName}' expects {expectedText} argument{suffix} but got {actualText}.", errorCode: "eval.function-arity");
    }

    public static MathSolverException FunctionUndefined(string functionName) =>
        new($"Function '{functionName}' is undefined for the provided arguments.", errorCode: "eval.function-domain");
}
