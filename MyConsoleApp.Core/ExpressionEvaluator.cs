using MyConsoleApp.Core.Errors;
using MyConsoleApp.Core.Parsing;

namespace MyConsoleApp.Core;

internal sealed class ExpressionEvaluator
{
    private readonly IMathEnvironment _environment;

    public ExpressionEvaluator()
        : this(new BuiltInMathEnvironment())
    {
    }

    public ExpressionEvaluator(IMathEnvironment environment)
    {
        _environment = environment;
    }

    public double Evaluate(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            throw MathSolverErrors.InputCannotBeEmpty();
        }

        var tokenizer = new Tokenizer(input);
        var tokens = tokenizer.Tokenize();
        var parser = new ExpressionParser(tokens, _environment);
        return parser.Parse();
    }
}
