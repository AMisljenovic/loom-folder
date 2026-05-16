using System.Globalization;
using MyConsoleApp.Core;

namespace MyConsoleApp.Cli;

internal sealed class ConsoleRunner
{
    private readonly MathSolver _solver;

    public ConsoleRunner(MathSolver solver)
    {
        _solver = solver;
    }

    public void Run()
    {
        WriteGreeting();

        while (true)
        {
            if (!TryReadInput(out var input))
            {
                break;
            }

            switch (TryHandleCommand(input))
            {
                case CommandResult.Exit:
                    return;
                case CommandResult.Handled:
                    continue;
                case CommandResult.NotHandled:
                    WriteResult(_solver.SolveSafely(input));
                    break;
            }
        }
    }

    private static void WriteGreeting()
    {
        Console.WriteLine("Interactive Math Solver");
        Console.WriteLine("Type an expression to evaluate it, or 'help' for usage instructions.");
    }

    private static bool TryReadInput(out string input)
    {
        Console.Write("> ");
        var rawInput = Console.ReadLine();

        if (rawInput is null)
        {
            input = string.Empty;
            return false;
        }

        input = rawInput.Trim();
        return true;
    }

    private static CommandResult TryHandleCommand(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return CommandResult.Handled;
        }

        if (input.Equals("exit", StringComparison.OrdinalIgnoreCase) ||
            input.Equals("quit", StringComparison.OrdinalIgnoreCase))
        {
            return CommandResult.Exit;
        }

        if (input.Equals("help", StringComparison.OrdinalIgnoreCase))
        {
            WriteHelp();
            return CommandResult.Handled;
        }

        return CommandResult.NotHandled;
    }

    private static void WriteResult(SolveResult result)
    {
        if (result.Success)
        {
            Console.WriteLine(result.Value.ToString("G", CultureInfo.InvariantCulture));
            return;
        }

        Console.WriteLine($"Error: {result.Error}");
    }

    private static void WriteHelp()
    {
        Console.WriteLine("Supported operators: +, -, *, /, ^");
        Console.WriteLine("Parentheses, constants, and functions are supported.");
        Console.WriteLine("Constants: pi, e");
        Console.WriteLine("Functions: sqrt, abs, sin, cos, tan, log, ln");
        Console.WriteLine("Examples:");
        Console.WriteLine("  2 + 2");
        Console.WriteLine("  (2 + 3) * 4");
        Console.WriteLine("  -2^2");
        Console.WriteLine("  sqrt(16) + 2*pi");
        Console.WriteLine("Commands: help, exit, quit");
    }

    private enum CommandResult
    {
        NotHandled,
        Handled,
        Exit
    }
}
