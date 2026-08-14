using System.Net.Http.Json;

namespace Zenvera.AiExamples.Applied.BlazorServerHost.Services;

public sealed class CatalogApiClient(HttpClient http)
{
    public async Task<List<Product>> GetAllAsync()
        => await http.GetFromJsonAsync<List<Product>>("/api/product") ?? [];

    public async Task<List<Product>> SearchAsync(string term, bool semantic)
    {
        var path = semantic
            ? $"/api/product/aisearch/{Uri.EscapeDataString(term)}"
            : $"/api/product/search/{Uri.EscapeDataString(term)}";
        return await http.GetFromJsonAsync<List<Product>>(path) ?? [];
    }

    public async Task<DiscoveryResponse?> DiscoverAsync(string question)
    {
        var response = await http.PostAsJsonAsync("/api/product/discover", new { Question = question });
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<DiscoveryResponse>();
    }
}

public sealed record DiscoveryResponse(string Answer, List<Product> Products);
