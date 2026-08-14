namespace Zenvera.AiExamples.Applied.Application.Contracts;

public interface IProductSemanticSearch
{
    Task InitializeAsync(IEnumerable<Product> products, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<int>> SearchAsync(string query, int maxResults = 3, CancellationToken cancellationToken = default);
}
