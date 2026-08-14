namespace Zenvera.AiExamples.Shared.Hosting;

public sealed class AiOptions
{
    public const string SectionName = "Ai";

    /// <summary>
    /// Mock (default, no network), Foundry (Azure OpenAI / Microsoft Foundry),
    /// or Ollama (OpenAI-compatible local endpoint, also used for Foundry Local).
    /// </summary>
    public string Provider { get; set; } = nameof(AiProvider.Mock);

    public string ChatModel { get; set; } = "gpt-5-mini";

    public string EmbeddingModel { get; set; } = "text-embedding-3-small";
}

public sealed class FoundryOptions
{
    public const string SectionName = "Foundry";

    public string? Endpoint { get; set; }

    public string? ApiKey { get; set; }
}

public sealed class OllamaOptions
{
    public const string SectionName = "Ollama";

    /// <summary>
    /// OpenAI-compatible base URL. Ollama default is http://localhost:11434/v1.
    /// Foundry Local uses the same shape with a different endpoint.
    /// </summary>
    public string Endpoint { get; set; } = "http://localhost:11434/v1";

    public string ApiKey { get; set; } = "ollama";

    public string ChatModel { get; set; } = "llama3.2";

    public string EmbeddingModel { get; set; } = "all-minilm";
}
