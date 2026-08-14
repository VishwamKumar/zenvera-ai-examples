namespace Zenvera.AiExamples.Shared.Hosting;

/// <summary>
/// Resolved chat and embedding clients for one example process.
/// </summary>
public sealed class AiClients : IDisposable
{
    public AiClients(
        AiProvider provider,
        IChatClient chatClient,
        IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator,
        int embeddingDimensions)
    {
        Provider = provider;
        ChatClient = chatClient;
        EmbeddingGenerator = embeddingGenerator;
        EmbeddingDimensions = embeddingDimensions;
    }

    public AiProvider Provider { get; }

    public IChatClient ChatClient { get; }

    public IEmbeddingGenerator<string, Embedding<float>> EmbeddingGenerator { get; }

    public int EmbeddingDimensions { get; }

    public void Dispose()
    {
        ChatClient.Dispose();
        EmbeddingGenerator.Dispose();
    }
}
