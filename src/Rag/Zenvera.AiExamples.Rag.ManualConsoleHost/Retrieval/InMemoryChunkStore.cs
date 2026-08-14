namespace Zenvera.AiExamples.Rag.ManualConsoleHost.Retrieval;

public sealed class InMemoryChunkStore
{
    private readonly List<(string Text, float[] Vector)> _chunks = [];

    public void Add(string text, float[] vector) => _chunks.Add((text, vector));

    public IReadOnlyList<string> Search(ReadOnlyMemory<float> query, int topK)
    {
        return _chunks
            .Select(chunk => (chunk.Text, Score: CosineSimilarity.Compute(query.Span, chunk.Vector)))
            .OrderByDescending(item => item.Score)
            .Take(topK)
            .Select(item => item.Text)
            .ToArray();
    }
}
