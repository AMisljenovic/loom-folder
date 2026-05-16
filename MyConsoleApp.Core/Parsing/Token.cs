namespace MyConsoleApp.Core.Parsing;

internal sealed record Token(TokenType Type, string Text, double? Number, int Position);
