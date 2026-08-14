namespace Zenvera.AiExamples.Shared.Hosting;

/// <summary>
/// Model provider selected by configuration. Foundry and Ollama share the same
/// <see cref="IChatClient"/> / <see cref="IEmbeddingGenerator{TInput,TEmbedding}"/>
/// application code; only registration changes.
/// </summary>
public enum AiProvider
{
    Mock,
    Foundry,
    Ollama
}
