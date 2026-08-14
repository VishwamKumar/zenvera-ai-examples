namespace Zenvera.AiExamples.Applied.Application.Contracts;

public interface IProductCatalog
{
    Task<IReadOnlyList<Product>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Product>> KeywordSearchAsync(string term, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Product>> GetByIdsAsync(IReadOnlyList<int> ids, CancellationToken cancellationToken = default);
}
