namespace Zenvera.AiExamples.Applied.Application.Discovery;

public sealed class ProductDiscoveryService(
    IProductSemanticSearch semanticSearch,
    IProductCatalog catalog,
    IChatClient chatClient) : IProductDiscovery
{
    private const string SystemPrompt = """
        You are a shopping assistant for Zenvera Outfitters.
        Answer using ONLY the products listed below. Recommend at most two.
        If none fit, say so plainly. Never invent products, prices, or features.
        Keep the answer under 80 words.
        """;

    public async Task<DiscoveryResult> AskAsync(string question, CancellationToken cancellationToken = default)
    {
        var ids = await semanticSearch.SearchAsync(question, cancellationToken: cancellationToken);
        var products = await catalog.GetByIdsAsync(ids, cancellationToken);

        if (products.Count == 0)
        {
            return new DiscoveryResult(
                "We do not stock anything that matches that. Try describing it differently.",
                products);
        }

        var catalogText = string.Join(
            Environment.NewLine,
            products.Select(product => $"- {product.Name} ({product.Price:C}): {product.Description}"));

        List<ChatMessage> messages =
        [
            new(ChatRole.System, SystemPrompt),
            new(ChatRole.User, $"Products available:\n{catalogText}\nShopper's question: {question}")
        ];

        var response = await chatClient.GetResponseAsync(messages, cancellationToken: cancellationToken);
        return new DiscoveryResult(response.Text, products);
    }
}
