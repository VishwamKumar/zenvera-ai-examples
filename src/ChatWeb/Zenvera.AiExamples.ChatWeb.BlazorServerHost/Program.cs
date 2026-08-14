using Zenvera.AiExamples.ChatWeb.BlazorServerHost.Components;

var builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults();
builder.Services.AddRazorComponents().AddInteractiveServerComponents();
builder.Services.AddZenveraAiClients(builder.Configuration);

var provider = AiProviderParser.Parse(builder.Configuration["Ai:Provider"]);
var qdrantAvailable = !string.IsNullOrWhiteSpace(builder.Configuration.GetConnectionString("vectordb"));
if (qdrantAvailable)
{
    builder.AddQdrantClient("vectordb");
}

builder.Services.AddChatWebInfrastructure(provider, qdrantAvailable);
builder.Services.AddHostedService<KnowledgeSeedHostedService>();

var app = builder.Build();
app.MapDefaultEndpoints();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseAntiforgery();
app.MapStaticAssets();
app.MapRazorComponents<App>().AddInteractiveServerRenderMode();
app.Run();

internal sealed class KnowledgeSeedHostedService(
    IKnowledgeIngestor ingestor,
    IWebHostEnvironment environment) : IHostedService
{
    public Task StartAsync(CancellationToken cancellationToken)
    {
        var directory = new DirectoryInfo(Path.Combine(environment.WebRootPath, "data"));
        return ingestor.IngestAsync(directory, cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
