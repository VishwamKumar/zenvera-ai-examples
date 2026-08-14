namespace Zenvera.AiExamples.Shared.Hosting;

public static class AiClientServiceCollectionExtensions
{
    public static IServiceCollection AddZenveraAiClients(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton(_ =>
        {
            var loggerFactory = _.GetService<ILoggerFactory>();
            return AiClientFactory.Create(configuration, loggerFactory);
        });
        services.AddSingleton(sp => sp.GetRequiredService<AiClients>().ChatClient);
        services.AddSingleton(sp => sp.GetRequiredService<AiClients>().EmbeddingGenerator);
        return services;
    }
}
