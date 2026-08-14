namespace Zenvera.AiExamples.Applied.Application.Discovery;

public sealed record DiscoveryResult(string Answer, IReadOnlyList<Product> Products);

public interface IProductDiscovery
{
    Task<DiscoveryResult> AskAsync(string question, CancellationToken cancellationToken = default);
}
