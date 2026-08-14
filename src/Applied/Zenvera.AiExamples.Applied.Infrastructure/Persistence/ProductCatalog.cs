namespace Zenvera.AiExamples.Applied.Infrastructure.Persistence;

public sealed class ProductCatalog(CatalogDbContext db) : IProductCatalog
{
    public async Task<IReadOnlyList<Product>> GetAllAsync(CancellationToken cancellationToken = default)
        => await db.Products.AsNoTracking().OrderBy(product => product.Id).ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<Product>> KeywordSearchAsync(string term, CancellationToken cancellationToken = default)
        => await db.Products.AsNoTracking()
            .Where(product => EF.Functions.Like(product.Name, $"%{term}%")
                || EF.Functions.Like(product.Description, $"%{term}%"))
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<Product>> GetByIdsAsync(IReadOnlyList<int> ids, CancellationToken cancellationToken = default)
    {
        var products = await db.Products.AsNoTracking()
            .Where(product => ids.Contains(product.Id))
            .ToListAsync(cancellationToken);

        return ids
            .Select(id => products.FirstOrDefault(product => product.Id == id))
            .OfType<Product>()
            .ToList();
    }
}
