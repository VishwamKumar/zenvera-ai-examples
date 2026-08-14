namespace Zenvera.AiExamples.Shared.Hosting;

public static class AiProviderParser
{
    public static AiProvider Parse(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return AiProvider.Mock;
        }

        if (Enum.TryParse<AiProvider>(value, ignoreCase: true, out var provider))
        {
            return provider;
        }

        throw new InvalidOperationException(
            $"Unknown Ai:Provider '{value}'. Use Mock, Foundry, or Ollama.");
    }
}
