using Microsoft.Extensions.Configuration;

var builder = DistributedApplication.CreateBuilder(args);
var provider = AiProviderParser.Parse(builder.Configuration["Ai:Provider"]);

var web = builder.AddProject<Projects.Zenvera_AiExamples_ChatWeb_BlazorServerHost>("chatweb")
    .WithEnvironment("Ai__Provider", provider.ToString())
    .WithExternalHttpEndpoints();

if (provider is AiProvider.Foundry)
{
    var openai = builder.AddConnectionString("openai");
    web.WithReference(openai);
}

if (provider is AiProvider.Ollama)
{
    var ollama = builder.AddOllama("ollama").WithDataVolume();
    var chat = ollama.AddModel("chat", builder.Configuration["Ollama:ChatModel"] ?? "llama3.2");
    var embeddings = ollama.AddModel("embeddings", builder.Configuration["Ollama:EmbeddingModel"] ?? "all-minilm");
    var endpoint = ollama.GetEndpoint("http");
    web.WithReference(chat)
        .WithReference(embeddings)
        .WaitFor(chat)
        .WaitFor(embeddings)
        .WithEnvironment("Ollama__Endpoint", endpoint);
}

if (provider is not AiProvider.Mock)
{
    var vectorDb = builder.AddQdrant("vectordb")
        .WithDataVolume()
        .WithLifetime(ContainerLifetime.Persistent);
    web.WithReference(vectorDb).WaitFor(vectorDb);
}

builder.Build().Run();
