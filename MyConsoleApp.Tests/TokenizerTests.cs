using MyConsoleApp.Core.Errors;
using MyConsoleApp.Core.Parsing;

namespace MyConsoleApp.Tests;

public class TokenizerTests
{
    [Fact]
    public void Tokenize_ReturnsExpectedTokens_ForExpression()
    {
        var tokenizer = new Tokenizer("12.5 + (3 * -2)");

        var tokens = tokenizer.Tokenize();

        Assert.Collection(
            tokens,
            token => AssertToken(token, TokenType.Number, "12.5", 12.5d, 0),
            token => AssertToken(token, TokenType.Plus, "+", null, 5),
            token => AssertToken(token, TokenType.LeftParen, "(", null, 7),
            token => AssertToken(token, TokenType.Number, "3", 3d, 8),
            token => AssertToken(token, TokenType.Star, "*", null, 10),
            token => AssertToken(token, TokenType.Minus, "-", null, 12),
            token => AssertToken(token, TokenType.Number, "2", 2d, 13),
            token => AssertToken(token, TokenType.RightParen, ")", null, 14),
            token => AssertToken(token, TokenType.End, string.Empty, null, 15));
    }

    [Fact]
    public void Tokenize_ReturnsExpectedTokens_ForFunctionsConstantsAndExponentiation()
    {
        var tokenizer = new Tokenizer("sqrt(pi^2) + abs(x_1,2)");

        var tokens = tokenizer.Tokenize();

        Assert.Collection(
            tokens,
            token => AssertToken(token, TokenType.Identifier, "sqrt", null, 0),
            token => AssertToken(token, TokenType.LeftParen, "(", null, 4),
            token => AssertToken(token, TokenType.Identifier, "pi", null, 5),
            token => AssertToken(token, TokenType.Caret, "^", null, 7),
            token => AssertToken(token, TokenType.Number, "2", 2d, 8),
            token => AssertToken(token, TokenType.RightParen, ")", null, 9),
            token => AssertToken(token, TokenType.Plus, "+", null, 11),
            token => AssertToken(token, TokenType.Identifier, "abs", null, 13),
            token => AssertToken(token, TokenType.LeftParen, "(", null, 16),
            token => AssertToken(token, TokenType.Identifier, "x_1", null, 17),
            token => AssertToken(token, TokenType.Comma, ",", null, 20),
            token => AssertToken(token, TokenType.Number, "2", 2d, 21),
            token => AssertToken(token, TokenType.RightParen, ")", null, 22),
            token => AssertToken(token, TokenType.End, string.Empty, null, 23));
    }

    [Theory]
    [InlineData("@", "Invalid character '@' at position 0.")]
    [InlineData(".", "Invalid number '.' at position 0.")]
    [InlineData("..", "Invalid number '.' at position 0.")]
    public void Tokenize_ThrowsMathSolverException_ForInvalidInput(string input, string expectedMessage)
    {
        var tokenizer = new Tokenizer(input);

        var exception = Assert.Throws<MathSolverException>(() => tokenizer.Tokenize());

        Assert.Equal(expectedMessage, exception.Message);
    }

    [Fact]
    public void Tokenize_ReturnsOnlyEndToken_ForWhitespaceOnlyInput()
    {
        var tokenizer = new Tokenizer("   \t  ");

        var tokens = tokenizer.Tokenize();

        Assert.Collection(tokens, token => AssertToken(token, TokenType.End, string.Empty, null, 6));
    }

    [Theory]
    [InlineData(".5", ".5", 0.5d)]
    [InlineData("5.", "5.", 5d)]
    public void Tokenize_ParsesNumbersWithSingleDecimalPoint(string input, string expectedText, double expectedValue)
    {
        var tokenizer = new Tokenizer(input);

        var tokens = tokenizer.Tokenize();

        Assert.Collection(
            tokens,
            token => AssertToken(token, TokenType.Number, expectedText, expectedValue, 0),
            token => AssertToken(token, TokenType.End, string.Empty, null, input.Length));
    }

    [Theory]
    [InlineData("_x")]
    [InlineData("a1")]
    [InlineData("A_B2")]
    public void Tokenize_ParsesIdentifiersContainingUnderscoresAndDigits(string input)
    {
        var tokenizer = new Tokenizer(input);

        var tokens = tokenizer.Tokenize();

        Assert.Collection(
            tokens,
            token => AssertToken(token, TokenType.Identifier, input, null, 0),
            token => AssertToken(token, TokenType.End, string.Empty, null, input.Length));
    }

    [Fact]
    public void Tokenize_SplitsAdjacentDecimalFragmentsIntoSeparateNumberTokens()
    {
        var tokenizer = new Tokenizer("1.2.3");

        var tokens = tokenizer.Tokenize();

        Assert.Collection(
            tokens,
            token => AssertToken(token, TokenType.Number, "1.2", 1.2d, 0),
            token => AssertToken(token, TokenType.Number, ".3", 0.3d, 3),
            token => AssertToken(token, TokenType.End, string.Empty, null, 5));
    }

    [Fact]
    public void Tokenize_PreservesOriginalCharacterPositions_AfterSkippingWhitespace()
    {
        var tokenizer = new Tokenizer("  _x + .5");

        var tokens = tokenizer.Tokenize();

        Assert.Collection(
            tokens,
            token => AssertToken(token, TokenType.Identifier, "_x", null, 2),
            token => AssertToken(token, TokenType.Plus, "+", null, 5),
            token => AssertToken(token, TokenType.Number, ".5", 0.5d, 7),
            token => AssertToken(token, TokenType.End, string.Empty, null, 9));
    }

    private static void AssertToken(Token token, TokenType expectedType, string expectedText, double? expectedNumber, int expectedPosition)
    {
        Assert.Equal(expectedType, token.Type);
        Assert.Equal(expectedText, token.Text);
        Assert.Equal(expectedNumber, token.Number);
        Assert.Equal(expectedPosition, token.Position);
    }
}
