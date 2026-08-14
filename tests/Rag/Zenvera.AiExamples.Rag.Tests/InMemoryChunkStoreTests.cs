namespace Zenvera.AiExamples.Rag.Tests;

public sealed class InMemoryChunkStoreTests
{
    [Fact]
    public void Search_returns_the_closest_paragraph()
    {
        var store = new InMemoryChunkStore();
        store.Add("Waterproof membrane RidgeDry 4-layer", MockEmbeddingGenerator.CreateVector("Waterproof membrane RidgeDry 4-layer"));
        store.Add("Return unworn boots within 60 days", MockEmbeddingGenerator.CreateVector("Return unworn boots within 60 days"));

        var query = MockEmbeddingGenerator.CreateVector("is the boot waterproof?");
        var results = store.Search(query, topK: 1);

        results.Should().ContainSingle();
        results[0].Should().Contain("RidgeDry");
    }
}
