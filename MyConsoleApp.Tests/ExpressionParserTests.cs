using MyConsoleApp.Core.Errors;
using MyConsoleApp.Core.Parsing;

namespace MyConsoleApp.Tests;

public class ExpressionParserTests
{
    [Theory]
    [InlineData("2+3*4", 14d)]
    [InlineData("(2+3)*4", 20d)]
    [InlineData("--2", 2d)]
    [InlineData("8/2/2", 2d)]
    [InlineData("2^3", 8d)]
    [InlineData("2^3^2", 512d)]
    [InlineData("2*3^2", 18d)]
    [InlineData("-2^2", -4d)]
    [InlineData("2^-2", 0.25d)]
    [InlineData("2^+3", 8d)]
    [InlineData("(-2)^2", 4d)]
    [InlineData("(-2)^3", -8d)]
    [InlineData("pi", Math.PI)]
    [InlineData("PI", Math.PI)]
    [InlineData("2*pi", 2d * Math.PI)]
    [InlineData("sqrt(9)", 3d)]
    [InlineData("SQRT(16)", 4d)]
    [InlineData("abs(-5)", 5d)]
    [InlineData("sin(0)", 0d)]
    [InlineData("cos(0)", 1d)]
    [InlineData("cos(pi)", -1d)]
    [InlineData("tan(0)", 0d)]
    [InlineData("log(100)", 2d)]
    [InlineData("log(10^2)", 2d)]
    [InlineData("ln(e)", 1d)]
    [InlineData("Ln(E)", 1d)]
    [InlineData("sqrt(abs(-9))", 3d)]
    [InlineData("2 + 3 * 4 ^ 2", 50d)]
    [InlineData("(2 + 3) * 4 ^ 2", 80d)]
    public void Parse_ReturnsExpectedResult_ForValidExpressions(string input, double expected)
    {
        var parser = CreateParser(input);

        var result = parser.Parse();

        Assert.Equal(expected, result, precision: 10);
    }

    [Fact]
    public void Parse_ReturnsApproximateZero_ForSinPi()
    {
        var parser = CreateParser("sin(pi)");

        var result = parser.Parse();

        Assert.Equal(0d, result, precision: 10);
    }

    [Theory]
    [InlineData("1/0", "Division by zero.")]
    [InlineData("1+", "Unexpected token '' at position 2.")]
    [InlineData("1)", "Unexpected token ')' at position 1.")]
    [InlineData("(1+2", "Missing closing parenthesis for '(' at position 0.")]
    [InlineData("foo", "Unknown identifier 'foo' at position 0.")]
    [InlineData("foo()", "Unknown function 'foo' at position 0.")]
    [InlineData("sqrt(", "Unexpected token '' at position 5.")]
    [InlineData("sqrt()", "Function 'sqrt' expects 1 argument but got 0.")]
    [InlineData("sqrt(1", "Expected ')' after arguments for function 'sqrt'.")]
    [InlineData("sqrt(1,2)", "Function 'sqrt' expects 1 argument but got 2.")]
    [InlineData("sqrt(1,)", "Unexpected token ')' at position 7.")]
    [InlineData("sqrt(,1)", "Unexpected token ',' at position 5.")]
    [InlineData("sqrt(1,,2)", "Unexpected token ',' at position 7.")]
    [InlineData("2/**3", "Unexpected token '*' at position 2.")]
    [InlineData("ln(0)", "Function 'ln' is undefined for the provided arguments.")]
    [InlineData("sqrt(-1)", "Function 'sqrt' is undefined for the provided arguments.")]
    [InlineData("log(0)", "Function 'log' is undefined for the provided arguments.")]
    [InlineData("pi foo", "Unexpected token 'foo' at position 3.")]
    [InlineData("()", "Unexpected token ')' at position 1.")]
    public void Parse_ThrowsMathSolverException_ForInvalidExpressions(string input, string expectedMessage)
    {
        var parser = CreateParser(input);

        var exception = Assert.Throws<MathSolverException>(() => parser.Parse());

        Assert.Equal(expectedMessage, exception.Message);
    }

    private static ExpressionParser CreateParser(string input)
    {
        var tokenizer = new Tokenizer(input);
        var tokens = tokenizer.Tokenize();
        return new ExpressionParser(tokens);
    }
}
