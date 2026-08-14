namespace Zenvera.AiExamples.Shared.Hosting;

public static class AiClientFactory
{
    public static AiClients Create(IConfiguration configuration, ILoggerFactory? loggerFactory = null)
    {
        var ai = configuration.GetSection(AiOptions.SectionName).Get<AiOptions>() ?? new AiOptions();
        var provider = AiProviderParser.Parse(ai.Provider);

        IChatClient chatClient;
        IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator;
        int dimensions;

        switch (provider)
        {
            case AiProvider.Mock:
                chatClient = new MockChatClient();
                embeddingGenerator = new MockEmbeddingGenerator();
                dimensions = MockEmbeddingGenerator.VectorSize;
                break;

            case AiProvider.Foundry:
            {
                var (endpoint, apiKey) = ResolveFoundry(configuration);
                var azure = new AzureOpenAIClient(new Uri(endpoint), new AzureKeyCredential(apiKey));
                chatClient = azure.GetChatClient(ai.ChatModel).AsIChatClient();
                embeddingGenerator = azure.GetEmbeddingClient(ai.EmbeddingModel).AsIEmbeddingGenerator();
                dimensions = 1536;
                break;
            }

            case AiProvider.Ollama:
            {
                var ollama = configuration.GetSection(OllamaOptions.SectionName).Get<OllamaOptions>()
                    ?? new OllamaOptions();
                var endpoint = ollama.Endpoint.TrimEnd('/');
                if (!endpoint.EndsWith("/v1", StringComparison.OrdinalIgnoreCase))
                {
                    endpoint += "/v1";
                }

                var openAi = new OpenAIClient(
                    new ApiKeyCredential(ollama.ApiKey),
                    new OpenAIClientOptions { Endpoint = new Uri(endpoint) });
                chatClient = openAi.GetChatClient(ollama.ChatModel).AsIChatClient();
                embeddingGenerator = openAi.GetEmbeddingClient(ollama.EmbeddingModel).AsIEmbeddingGenerator();
                dimensions = 384;
                break;
            }

            default:
                throw new InvalidOperationException($"Unsupported provider '{provider}'.");
        }

        if (loggerFactory is not null && provider is not AiProvider.Mock)
        {
            chatClient = chatClient.AsBuilder().UseLogging(loggerFactory).Build();
        }

        return new AiClients(provider, chatClient, embeddingGenerator, dimensions);
    }

    private static (string Endpoint, string ApiKey) ResolveFoundry(IConfiguration configuration)
    {
        var foundry = configuration.GetSection(FoundryOptions.SectionName).Get<FoundryOptions>()
            ?? new FoundryOptions();

        var endpoint = foundry.Endpoint;
        var apiKey = foundry.ApiKey;

        var connectionString = configuration.GetConnectionString("openai");
        if (!string.IsNullOrWhiteSpace(connectionString))
        {
            endpoint ??= ReadConnectionValue(connectionString, "Endpoint");
            apiKey ??= ReadConnectionValue(connectionString, "Key")
                ?? ReadConnectionValue(connectionString, "ApiKey");
        }

        if (string.IsNullOrWhiteSpace(endpoint) || string.IsNullOrWhiteSpace(apiKey))
        {
            throw new InvalidOperationException(
                "Foundry requires Foundry:Endpoint and Foundry:ApiKey (user secrets) " +
                "or ConnectionStrings:openai (Endpoint=...;Key=...).");
        }

        return (endpoint, apiKey);
    }

    private static string? ReadConnectionValue(string connectionString, string key)
    {
        foreach (var part in connectionString.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            var separator = part.IndexOf('=');
            if (separator <= 0)
            {
                continue;
            }

            if (part[..separator].Equals(key, StringComparison.OrdinalIgnoreCase))
            {
                return part[(separator + 1)..];
            }
        }

        return null;
    }
}
