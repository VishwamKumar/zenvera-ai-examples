namespace Zenvera.AiExamples.ChatWeb.Infrastructure.Ingestion;

public sealed class MarkdownKnowledgeIngestor(
    IChunkStore store,
    IEmbeddingGenerator<string, Embedding<float>> embeddings,
    ILogger<MarkdownKnowledgeIngestor> logger) : IKnowledgeIngestor
{
    public async Task IngestAsync(DirectoryInfo directory, CancellationToken cancellationToken = default)
    {
        var files = directory.Exists
            ? directory.GetFiles("*.md", SearchOption.AllDirectories)
            : [];

        var chunks = new List<KnowledgeChunk>();
        foreach (var file in files)
        {
            var text = await File.ReadAllTextAsync(file.FullName, cancellationToken);
            var paragraphs = text
                .ReplaceLineEndings("\n")
                .Split("\n\n", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            var vectors = await embeddings.GenerateAsync(paragraphs, cancellationToken: cancellationToken);
            for (var i = 0; i < paragraphs.Length; i++)
            {
                chunks.Add(new KnowledgeChunk(file.Name, paragraphs[i], vectors[i].Vector.ToArray()));
            }
        }

        await store.UpsertAsync(chunks, cancellationToken);
        logger.LogInformation("Ingested {ChunkCount} chunks from {FileCount} markdown files.", chunks.Count, files.Length);
    }
}
