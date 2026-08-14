namespace Zenvera.AiExamples.ChatWeb.Infrastructure.VectorStore;

public sealed class QdrantChunkStore(QdrantClient client, AiClients aiClients) : IChunkStore
{
    public const string CollectionName = "chatweb-chunks";

    public async Task UpsertAsync(IReadOnlyList<KnowledgeChunk> chunks, CancellationToken cancellationToken = default)
    {
        var exists = await client.CollectionExistsAsync(CollectionName, cancellationToken);
        if (!exists)
        {
            await client.CreateCollectionAsync(
                CollectionName,
                new VectorParams
                {
                    Size = (ulong)aiClients.EmbeddingDimensions,
                    Distance = Distance.Cosine
                },
                cancellationToken: cancellationToken);
        }

        var points = chunks.Select(chunk => new PointStruct
        {
            Id = Guid.NewGuid(),
            Vectors = chunk.Vector,
            Payload =
            {
                ["text"] = chunk.Text,
                ["documentId"] = chunk.DocumentId
            }
        }).ToList();

        await client.UpsertAsync(CollectionName, points, cancellationToken: cancellationToken);
    }

    public async Task<IReadOnlyList<KnowledgeChunk>> SearchAsync(
        float[] query,
        int topK,
        CancellationToken cancellationToken = default)
    {
        var results = await client.SearchAsync(
            CollectionName,
            query,
            limit: (uint)topK,
            cancellationToken: cancellationToken);

        return results.Select(result => new KnowledgeChunk(
            result.Payload.TryGetValue("documentId", out var documentId) ? documentId.StringValue : "unknown",
            result.Payload.TryGetValue("text", out var text) ? text.StringValue : string.Empty,
            query)).ToArray();
    }
}
