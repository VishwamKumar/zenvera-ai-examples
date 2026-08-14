namespace Zenvera.AiExamples.Applied.Infrastructure;

public static class AppliedInfrastructureExtensions
{
    public static IServiceCollection AddAppliedInfrastructure(this IServiceCollection services, string sqlitePath)
    {
        services.AddDbContext<CatalogDbContext>(options => options.UseSqlite($"Data Source={sqlitePath}"));
        services.AddScoped<IProductCatalog, ProductCatalog>();
        services.AddSingleton<IProductSemanticSearch, InMemoryProductSemanticSearch>();
        services.AddScoped<IProductDiscovery, ProductDiscoveryService>();
        return services;
    }
}
