namespace Zenvera.AiExamples.ChatWeb.Application.Contracts;

public interface IChunkStore
{
    Task UpsertAsync(IReadOnlyList<KnowledgeChunk> chunks, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<KnowledgeChunk>> SearchAsync(
        float[] query,
        int topK,
        CancellationToken cancellationToken = default);
}
