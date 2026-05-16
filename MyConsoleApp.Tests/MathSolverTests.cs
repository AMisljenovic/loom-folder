﻿﻿﻿﻿﻿using System.Globalization;
using MyConsoleApp.Core;
using MyConsoleApp.Core.Errors;

namespace MyConsoleApp.Tests;

public class MathSolverTests
{
    private readonly MathSolver _solver = new();

    [Theory]
    [InlineData("2+2", 4d)]
    [InlineData("2+3*4", 14d)]
    [InlineData("(2+3)*4", 20d)]
    [InlineData("-5+2", -3d)]
    [InlineData("8/2", 4d)]
    [InlineData("2 * -4", -8d)]
    [InlineData("3.5 + 0.5", 4d)]
    [InlineData("((2+3))", 5d)]
    [InlineData("2 * (3 + 4) / 7", 2d)]
    [InlineData("--2", 2d)]
    [InlineData("2^3^2", 512d)]
    [InlineData("-2^2", -4d)]
    [InlineData("sqrt(9)", 3d)]
    [InlineData("abs(-5)", 5d)]
    [InlineData("sin(0)", 0d)]
    [InlineData("cos(0)", 1d)]
    [InlineData("tan(0)", 0d)]
    [InlineData("log(100)", 2d)]
    [InlineData("ln(e)", 1d)]
    [InlineData("2*pi", 2d * Math.PI)]
    public void SolveSafely_ReturnsExpectedResult_ForValidExpressions(string input, double expected)
    {
        var result = _solver.SolveSafely(input);

        Assert.True(result.Success);
        Assert.Null(result.Error);
        Assert.Equal(expected, result.Value, precision: 10);
    }

    [Theory]
    [InlineData("", "Input cannot be empty.")]
    [InlineData("   ", "Input cannot be empty.")]
    [InlineData("1/0", "Division by zero.")]
    [InlineData("2+*3", "Unexpected token '*' at position 2.")]
    [InlineData("(1+2", "Missing closing parenthesis for '(' at position 0.")]
    [InlineData("abc", "Unknown identifier 'abc' at position 0.")]
    [InlineData(".", "Invalid number '.' at position 0.")]
    [InlineData("1+", "Unexpected token '' at position 2.")]
    [InlineData("1)", "Unexpected token ')' at position 1.")]
    [InlineData("2..3", "Unexpected token '.3' at position 2.")]
    [InlineData("foo()", "Unknown function 'foo' at position 0.")]
    [InlineData("sqrt(1,2)", "Function 'sqrt' expects 1 argument but got 2.")]
    public void SolveSafely_ReturnsError_ForInvalidExpressions(string input, string expectedError)
    {
        var result = _solver.SolveSafely(input);

        Assert.False(result.Success);
        Assert.Equal(0d, result.Value);
        Assert.Equal(expectedError, result.Error);
    }

    [Fact]
    public void SolveSafely_ReturnsExpectedResult_ForCaseInsensitiveFunctionAndConstantExpression()
    {
        var result = _solver.SolveSafely("SQRT(PI^2)");

        Assert.True(result.Success);
        Assert.Null(result.Error);
        Assert.Equal(Math.PI, result.Value, precision: 10);
    }

    [Fact]
    public void SolveSafely_ReturnsExpectedResult_ForWhitespaceHeavyExpression()
    {
        var result = _solver.SolveSafely("  \t( 2 + 3 ) * 4 \r\n");

        Assert.True(result.Success);
        Assert.Null(result.Error);
        Assert.Equal(20d, result.Value, precision: 10);
    }

    [Fact]
    public void Solve_ReturnsExpectedResult_ForValidExpression()
    {
        var result = _solver.Solve("sqrt(9) + 3.5 + 0.5");

        Assert.Equal(7d, result, precision: 10);
    }

    [Fact]
    public void Solve_ThrowsMathSolverException_ForInvalidExpression()
    {
        var exception = Assert.Throws<MathSolverException>(() => _solver.Solve("1/0"));

        Assert.Equal("Division by zero.", exception.Message);
    }

    [Fact]
    public void Solve_ThrowsMathSolverException_ForUndefinedFunctionDomain()
    {
        var exception = Assert.Throws<MathSolverException>(() => _solver.Solve("sqrt(-1)"));

        Assert.Equal("Function 'sqrt' is undefined for the provided arguments.", exception.Message);
    }

    [Fact]
    public void TrySolve_RemainsAvailable_ForCompatibility()
    {
#pragma warning disable CS0618
        var success = _solver.TrySolve("(2 + 3) * sqrt(16)", out var value, out var error);
#pragma warning restore CS0618

        Assert.True(success);
        Assert.Equal(20d, value, precision: 10);
        Assert.Null(error);
    }

    [Fact]
    public void SolveSafely_ReturnsValue_ForValidExpression()
    {
        var result = _solver.SolveSafely("(2 + 3) * sqrt(16)");

        Assert.True(result.Success);
        Assert.Equal(20d, result.Value, precision: 10);
        Assert.Null(result.Error);
    }

    [Fact]
    public void SolveSafely_ReturnsError_ForInvalidExpression()
    {
        var result = _solver.SolveSafely("foo");

        Assert.False(result.Success);
        Assert.Equal(0d, result.Value);
        Assert.Equal("Unknown identifier 'foo' at position 0.", result.Error);
    }

    [Fact]
    public void SolveSafely_ReturnsError_ForEmptyInput()
    {
        var result = _solver.SolveSafely("   ");

        Assert.False(result.Success);
        Assert.Equal(0d, result.Value);
        Assert.Equal("Input cannot be empty.", result.Error);
    }

    [Fact]
    public void SolveSafely_ReturnsError_ForUndefinedFunctionDomain()
    {
        var result = _solver.SolveSafely("sqrt(-1)");

        Assert.False(result.Success);
        Assert.Equal(0d, result.Value);
        Assert.Equal("Function 'sqrt' is undefined for the provided arguments.", result.Error);
    }

    [Fact]
    public void Solve_UsesInvariantCultureParsing()
    {
        var originalCulture = CultureInfo.CurrentCulture;
        var originalUiCulture = CultureInfo.CurrentUICulture;

        try
        {
            var culture = CultureInfo.GetCultureInfo("fr-FR");
            CultureInfo.CurrentCulture = culture;
            CultureInfo.CurrentUICulture = culture;

            var result = _solver.Solve("sqrt(9) + 3.5 + 0.5");

            Assert.Equal(7d, result, precision: 10);
        }
        finally
        {
            CultureInfo.CurrentCulture = originalCulture;
            CultureInfo.CurrentUICulture = originalUiCulture;
        }
    }
}
