namespace Zenvera.AiExamples.Applied.Infrastructure.Search;

public sealed class InMemoryProductSemanticSearch(
    IEmbeddingGenerator<string, Embedding<float>> embeddings) : IProductSemanticSearch
{
    private readonly List<(int Id, float[] Vector)> _index = [];

    public async Task InitializeAsync(IEnumerable<Product> products, CancellationToken cancellationToken = default)
    {
        _index.Clear();
        var material = products.ToArray();
        var sources = material.Select(product => $"{product.Name}. {product.Description}").ToArray();
        var vectors = await embeddings.GenerateAsync(sources, cancellationToken: cancellationToken);
        for (var i = 0; i < material.Length; i++)
        {
            _index.Add((material[i].Id, vectors[i].Vector.ToArray()));
        }
    }

    public async Task<IReadOnlyList<int>> SearchAsync(
        string query,
        int maxResults = 3,
        CancellationToken cancellationToken = default)
    {
        var queryVector = await embeddings.GenerateAsync(query, cancellationToken: cancellationToken);
        return _index
            .Select(item => (item.Id, Score: VectorSimilarity.Cosine(queryVector.Vector.Span, item.Vector)))
            .Where(item => item.Score >= 0.15f)
            .OrderByDescending(item => item.Score)
            .Take(maxResults)
            .Select(item => item.Id)
            .ToArray();
    }
}
