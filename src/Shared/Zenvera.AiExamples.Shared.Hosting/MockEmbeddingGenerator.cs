namespace Zenvera.AiExamples.Shared.Hosting;

/// <summary>
/// Deterministic hash embedding so RAG examples retrieve by overlapping tokens without an API key.
/// </summary>
public sealed class MockEmbeddingGenerator : IEmbeddingGenerator<string, Embedding<float>>
{
    public const int VectorSize = 128;

    public Task<GeneratedEmbeddings<Embedding<float>>> GenerateAsync(
        IEnumerable<string> values,
        EmbeddingGenerationOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var embeddings = values.Select(value => new Embedding<float>(CreateVector(value))).ToArray();
        return Task.FromResult(new GeneratedEmbeddings<Embedding<float>>(embeddings));
    }

    public object? GetService(Type serviceType, object? serviceKey = null) => null;

    public void Dispose()
    {
    }

    public static float[] CreateVector(string text)
    {
        var vector = new float[VectorSize];
        var tokens = text.ToLowerInvariant()
            .Split([' ', '\n', '\r', '\t', '.', ',', ';', ':', '!', '?', '(', ')', '[', ']', '{', '}', '"', '\''],
                StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        foreach (var token in tokens)
        {
            var hash = 17;
            foreach (var character in token)
            {
                hash = (hash * 31) + character;
            }

            vector[Math.Abs(hash % VectorSize)] += 1f;
        }

        var magnitude = MathF.Sqrt(vector.Sum(value => value * value));
        if (magnitude <= 0)
        {
            return vector;
        }

        for (var i = 0; i < vector.Length; i++)
        {
            vector[i] /= magnitude;
        }

        return vector;
    }
}
