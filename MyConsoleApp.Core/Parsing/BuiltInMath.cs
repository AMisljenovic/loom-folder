namespace MyConsoleApp.Core.Parsing;

internal static class BuiltInMath
{
    private static readonly IMathEnvironment Environment = new BuiltInMathEnvironment();

    public static double ResolveConstant(Token identifier) => Environment.ResolveConstant(identifier);

    public static double InvokeFunction(Token identifier, IReadOnlyList<double> arguments) => Environment.InvokeFunction(identifier, arguments);
}
