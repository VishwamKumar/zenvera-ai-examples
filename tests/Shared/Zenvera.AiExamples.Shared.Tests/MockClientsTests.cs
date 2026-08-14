namespace Zenvera.AiExamples.Shared.Tests;

public sealed class MockClientsTests
{
    [Fact]
    public async Task Mock_chat_quotes_retrieved_context()
    {
        var client = new MockChatClient();
        var response = await client.GetResponseAsync(
        [
            new ChatMessage(ChatRole.System, "Context:\nWaterproof membrane RidgeDry"),
            new ChatMessage(ChatRole.User, "Is it waterproof?")
        ]);

        response.Text.Should().Contain("RidgeDry");
    }

    [Fact]
    public async Task Mock_embeddings_are_stable_for_the_same_text()
    {
        var generator = new MockEmbeddingGenerator();
        var first = await generator.GenerateAsync(["ridgewalker trail boot"]);
        var second = await generator.GenerateAsync(["ridgewalker trail boot"]);
        first[0].Vector.ToArray().Should().Equal(second[0].Vector.ToArray());
    }

    [Fact]
    public void Factory_defaults_to_mock()
    {
        var configuration = new ConfigurationBuilder().AddInMemoryCollection().Build();
        using var clients = AiClientFactory.Create(configuration);
        clients.Provider.Should().Be(AiProvider.Mock);
        clients.EmbeddingDimensions.Should().Be(MockEmbeddingGenerator.VectorSize);
    }
}
