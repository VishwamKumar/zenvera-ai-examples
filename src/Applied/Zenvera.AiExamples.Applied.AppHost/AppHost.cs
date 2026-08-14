var builder = DistributedApplication.CreateBuilder(args);

var api = builder.AddProject<Projects.Zenvera_AiExamples_Applied_RestApiHost>("catalog-api")
    .WithEnvironment("Ai__Provider", builder.Configuration["Ai:Provider"] ?? "Mock");

builder.AddProject<Projects.Zenvera_AiExamples_Applied_BlazorServerHost>("storefront")
    .WithReference(api)
    .WaitFor(api)
    .WithExternalHttpEndpoints();

builder.Build().Run();
