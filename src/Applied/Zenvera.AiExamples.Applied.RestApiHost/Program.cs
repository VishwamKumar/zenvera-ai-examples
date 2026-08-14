var builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults();
builder.Services.AddOpenApi();
builder.Services.AddZenveraAiClients(builder.Configuration);

var sqlitePath = Path.Combine(builder.Environment.ContentRootPath, "catalog.db");
builder.Services.AddAppliedInfrastructure(sqlitePath);

var app = builder.Build();
app.MapDefaultEndpoints();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<CatalogDbContext>();
    await db.Database.EnsureCreatedAsync();
    await CatalogSeed.EnsureSeededAsync(db);
    var products = await scope.ServiceProvider.GetRequiredService<IProductCatalog>().GetAllAsync();
    await scope.ServiceProvider.GetRequiredService<IProductSemanticSearch>().InitializeAsync(products);
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options => options.SwaggerEndpoint("/openapi/v1.json", "Applied catalog"));
}

app.MapGet("/api/product", async (IProductCatalog catalog) => await catalog.GetAllAsync());
app.MapGet("/api/product/search/{term}", async (string term, IProductCatalog catalog)
    => await catalog.KeywordSearchAsync(term));
app.MapGet("/api/product/aisearch/{term}", async (
    string term,
    IProductSemanticSearch semanticSearch,
    IProductCatalog catalog) =>
{
    var ids = await semanticSearch.SearchAsync(term);
    return await catalog.GetByIdsAsync(ids);
});
app.MapPost("/api/product/discover", async (DiscoveryRequest request, IProductDiscovery discovery)
    => await discovery.AskAsync(request.Question));

app.Run();

internal sealed record DiscoveryRequest(string Question);
