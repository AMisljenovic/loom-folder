using System.Globalization;
using MyConsoleApp.Core.Errors;

namespace MyConsoleApp.Core.Parsing;

internal sealed class Tokenizer(string input)
{
    private readonly string _input = input;
    private int _position;

    public List<Token> Tokenize()
    {
        var tokens = new List<Token>();

        while (!IsAtEnd())
        {
            var current = _input[_position];

            if (char.IsWhiteSpace(current))
            {
                _position++;
                continue;
            }

            if (char.IsDigit(current) || current == '.')
            {
                tokens.Add(ReadNumber());
                continue;
            }

            if (char.IsLetter(current) || current == '_')
            {
                tokens.Add(ReadIdentifier());
                continue;
            }

            var token = current switch
            {
                '+' => new Token(TokenType.Plus, "+", null, _position),
                '-' => new Token(TokenType.Minus, "-", null, _position),
                '*' => new Token(TokenType.Star, "*", null, _position),
                '/' => new Token(TokenType.Slash, "/", null, _position),
                '^' => new Token(TokenType.Caret, "^", null, _position),
                ',' => new Token(TokenType.Comma, ",", null, _position),
                '(' => new Token(TokenType.LeftParen, "(", null, _position),
                ')' => new Token(TokenType.RightParen, ")", null, _position),
                _ => throw MathSolverErrors.InvalidCharacter(current, _position)
            };

            tokens.Add(token);
            _position++;
        }

        tokens.Add(new Token(TokenType.End, string.Empty, null, _position));
        return tokens;
    }

    private Token ReadNumber()
    {
        var start = _position;
        var decimalPoints = 0;

        while (!IsAtEnd())
        {
            var current = _input[_position];

            if (current == '.')
            {
                decimalPoints++;
                if (decimalPoints > 1)
                {
                    break;
                }

                _position++;
                continue;
            }

            if (!char.IsDigit(current))
            {
                break;
            }

            _position++;
        }

        var text = _input[start.._position];

        if (text == "." || !double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out var value))
        {
            throw MathSolverErrors.InvalidNumber(text, start);
        }

        return new Token(TokenType.Number, text, value, start);
    }

    private Token ReadIdentifier()
    {
        var start = _position;

        while (!IsAtEnd())
        {
            var current = _input[_position];
            if (!char.IsLetterOrDigit(current) && current != '_')
            {
                break;
            }

            _position++;
        }

        var text = _input[start.._position];
        return new Token(TokenType.Identifier, text, null, start);
    }

    private bool IsAtEnd() => _position >= _input.Length;
}
