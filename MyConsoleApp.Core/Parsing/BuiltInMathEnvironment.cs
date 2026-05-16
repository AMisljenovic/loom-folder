using System.Globalization;
using MyConsoleApp.Core.Errors;

namespace MyConsoleApp.Core.Parsing;

internal interface IMathEnvironment
{
    double ResolveConstant(Token identifier);

    double InvokeFunction(Token identifier, IReadOnlyList<double> arguments);
}

internal sealed class BuiltInMathEnvironment : IMathEnvironment
{
    private static readonly Dictionary<string, double> Constants = new(StringComparer.OrdinalIgnoreCase)
    {
        ["pi"] = Math.PI,
        ["e"] = Math.E
    };

    private static readonly Dictionary<string, FunctionDefinition> Functions = new(StringComparer.OrdinalIgnoreCase)
    {
        ["sqrt"] = new FunctionDefinition("sqrt", 1, args => ValidateFinite(Math.Sqrt(args[0]), "sqrt")),
        ["abs"] = new FunctionDefinition("abs", 1, args => Math.Abs(args[0])),
        ["sin"] = new FunctionDefinition("sin", 1, args => Math.Sin(args[0])),
        ["cos"] = new FunctionDefinition("cos", 1, args => Math.Cos(args[0])),
        ["tan"] = new FunctionDefinition("tan", 1, args => Math.Tan(args[0])),
        ["log"] = new FunctionDefinition("log", 1, args => ValidateFinite(Math.Log10(args[0]), "log")),
        ["ln"] = new FunctionDefinition("ln", 1, args => ValidateFinite(Math.Log(args[0]), "ln"))
    };

    public double ResolveConstant(Token identifier)
    {
        if (Constants.TryGetValue(identifier.Text, out var value))
        {
            return value;
        }

        throw MathSolverErrors.UnknownIdentifier(identifier);
    }

    public double InvokeFunction(Token identifier, IReadOnlyList<double> arguments)
    {
        if (!Functions.TryGetValue(identifier.Text, out var definition))
        {
            throw MathSolverErrors.UnknownFunction(identifier);
        }

        if (arguments.Count != definition.Arity)
        {
            throw MathSolverErrors.FunctionArityMismatch(definition.Name, definition.Arity, arguments.Count);
        }

        return definition.Evaluate(arguments);
    }

    private static double ValidateFinite(double value, string functionName)
    {
        if (double.IsNaN(value) || double.IsInfinity(value))
        {
            throw MathSolverErrors.FunctionUndefined(functionName);
        }

        return value;
    }

    private sealed record FunctionDefinition(string Name, int Arity, Func<IReadOnlyList<double>, double> Evaluate);
}
