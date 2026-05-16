using MyConsoleApp.Core.Errors;

namespace MyConsoleApp.Core.Parsing;

internal sealed class ExpressionParser
{
    private readonly IMathEnvironment _environment;
    private readonly IReadOnlyList<Token> _tokens;
    private int _index;

    public ExpressionParser(IReadOnlyList<Token> tokens)
        : this(tokens, new BuiltInMathEnvironment())
    {
    }

    public ExpressionParser(IReadOnlyList<Token> tokens, IMathEnvironment environment)
    {
        _tokens = tokens;
        _environment = environment;
    }

    public double Parse()
    {
        var result = ParseExpression();

        if (Current.Type != TokenType.End)
        {
            throw MathSolverErrors.UnexpectedToken(Current);
        }

        return result;
    }

    private double ParseExpression()
    {
        var value = ParseTerm();

        while (Current.Type is TokenType.Plus or TokenType.Minus)
        {
            var operatorToken = Current;
            Advance();
            var right = ParseTerm();
            value = operatorToken.Type == TokenType.Plus ? value + right : value - right;
        }

        return value;
    }

    private double ParseTerm()
    {
        var value = ParseUnary();

        while (Current.Type is TokenType.Star or TokenType.Slash)
        {
            var operatorToken = Current;
            Advance();
            var right = ParseUnary();

            if (operatorToken.Type == TokenType.Star)
            {
                value *= right;
                continue;
            }

            if (right == 0)
            {
                throw MathSolverErrors.DivisionByZero();
            }

            value /= right;
        }

        return value;
    }

    private double ParseUnary()
    {
        if (Current.Type == TokenType.Plus)
        {
            Advance();
            return ParseUnary();
        }

        if (Current.Type == TokenType.Minus)
        {
            Advance();
            return -ParseUnary();
        }

        return ParsePower();
    }

    private double ParsePower()
    {
        var left = ParsePrimary();

        if (Current.Type == TokenType.Caret)
        {
            Advance();
            var right = ParsePowerOperand();
            return Math.Pow(left, right);
        }

        return left;
    }

    private double ParsePowerOperand()
    {
        if (Current.Type == TokenType.Plus)
        {
            Advance();
            return ParsePowerOperand();
        }

        if (Current.Type == TokenType.Minus)
        {
            Advance();
            return -ParsePowerOperand();
        }

        return ParsePower();
    }

    private double ParsePrimary()
    {
        if (Current.Type == TokenType.Number)
        {
            var value = Current.Number ?? throw MathSolverErrors.MissingNumberValue(Current);
            Advance();
            return value;
        }

        if (Current.Type == TokenType.Identifier)
        {
            return ParseIdentifier();
        }

        if (Current.Type == TokenType.LeftParen)
        {
            var openingPosition = Current.Position;
            Advance();
            var value = ParseExpression();

            if (Current.Type != TokenType.RightParen)
            {
                throw MathSolverErrors.MissingClosingParenthesis(openingPosition);
            }

            Advance();
            return value;
        }

        throw MathSolverErrors.UnexpectedToken(Current);
    }

    private double ParseIdentifier()
    {
        var identifier = Current;
        Advance();

        if (Current.Type != TokenType.LeftParen)
        {
            return _environment.ResolveConstant(identifier);
        }

        Advance();
        var arguments = new List<double>();

        if (Current.Type != TokenType.RightParen)
        {
            while (true)
            {
                arguments.Add(ParseExpression());

                if (Current.Type == TokenType.Comma)
                {
                    Advance();
                    continue;
                }

                break;
            }
        }

        if (Current.Type != TokenType.RightParen)
        {
            throw MathSolverErrors.ExpectedClosingParenthesisForFunction(identifier.Text);
        }

        Advance();
        return _environment.InvokeFunction(identifier, arguments);
    }

    private Token Current => _tokens[_index];

    private void Advance()
    {
        if (_index < _tokens.Count - 1)
        {
            _index++;
        }
    }
}
