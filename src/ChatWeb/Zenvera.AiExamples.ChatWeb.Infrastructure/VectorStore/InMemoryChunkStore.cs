namespace Zenvera.AiExamples.ChatWeb.Infrastructure.VectorStore;

public sealed class InMemoryChunkStore : IChunkStore
{
    private readonly List<KnowledgeChunk> _chunks = [];
    private readonly object _gate = new();

    public Task UpsertAsync(IReadOnlyList<KnowledgeChunk> chunks, CancellationToken cancellationToken = default)
    {
        lock (_gate)
        {
            _chunks.Clear();
            _chunks.AddRange(chunks);
        }

        return Task.CompletedTask;
    }

    public Task<IReadOnlyList<KnowledgeChunk>> SearchAsync(
        float[] query,
        int topK,
        CancellationToken cancellationToken = default)
    {
        List<KnowledgeChunk> snapshot;
        lock (_gate)
        {
            snapshot = [.. _chunks];
        }

        IReadOnlyList<KnowledgeChunk> ranked = snapshot
            .Select(chunk => (chunk, Score: VectorSimilarity.Cosine(query, chunk.Vector)))
            .OrderByDescending(item => item.Score)
            .Take(topK)
            .Select(item => item.chunk)
            .ToArray();

        return Task.FromResult(ranked);
    }
}
