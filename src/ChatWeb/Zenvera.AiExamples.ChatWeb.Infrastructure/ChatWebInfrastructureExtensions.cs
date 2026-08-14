namespace Zenvera.AiExamples.ChatWeb.Infrastructure;

public static class ChatWebInfrastructureExtensions
{
    public static IServiceCollection AddChatWebInfrastructure(
        this IServiceCollection services,
        AiProvider provider,
        bool qdrantAvailable)
    {
        if (provider is not AiProvider.Mock && qdrantAvailable)
        {
            services.AddSingleton<IChunkStore, QdrantChunkStore>();
        }
        else
        {
            services.AddSingleton<IChunkStore, InMemoryChunkStore>();
        }

        services.AddSingleton<IKnowledgeIngestor, MarkdownKnowledgeIngestor>();
        services.AddSingleton<GroundedChatService>();
        return services;
    }
}
