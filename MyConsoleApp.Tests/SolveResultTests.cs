using MyConsoleApp.Core;

namespace MyConsoleApp.Tests;

public class SolveResultTests
{
    [Fact]
    public void FromValue_ReturnsSuccessfulResult()
    {
        var result = SolveResult.FromValue(42d);

        Assert.True(result.Success);
        Assert.Equal(42d, result.Value);
        Assert.Null(result.Error);
    }

    [Fact]
    public void FromError_ReturnsFailedResult()
    {
        var result = SolveResult.FromError("boom");

        Assert.False(result.Success);
        Assert.Equal(0d, result.Value);
        Assert.Equal("boom", result.Error);
    }

    [Fact]
    public void Deconstruct_ReturnsExpectedFields()
    {
        var result = SolveResult.FromError("boom");

        var (success, value, error) = result;

        Assert.False(success);
        Assert.Equal(0d, value);
        Assert.Equal("boom", error);
    }
}
