namespace Zenvera.AiExamples.Shared.Tests;

public sealed class AiProviderParserTests
{
    [Theory]
    [InlineData(null, AiProvider.Mock)]
    [InlineData("", AiProvider.Mock)]
    [InlineData("mock", AiProvider.Mock)]
    [InlineData("Foundry", AiProvider.Foundry)]
    [InlineData("OLLAMA", AiProvider.Ollama)]
    public void Parse_known_values(string? value, AiProvider expected)
        => AiProviderParser.Parse(value).Should().Be(expected);

    [Fact]
    public void Parse_rejects_unknown_values()
    {
        var act = () => AiProviderParser.Parse("azure-openai");
        act.Should().Throw<InvalidOperationException>();
    }
}
