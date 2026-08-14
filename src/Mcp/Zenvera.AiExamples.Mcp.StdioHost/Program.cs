var builder = Host.CreateApplicationBuilder(args);

builder.Logging.AddConsole(options =>
{
    options.LogToStandardErrorThreshold = LogLevel.Trace;
});

builder.Services
    .AddMcpServer()
    .WithStdioServerTransport()
    .WithTools<Zenvera.AiExamples.Mcp.StdioHost.Tools.RandomNumberTools>()
    .WithTools<Zenvera.AiExamples.Mcp.StdioHost.Tools.WeatherTools>();

await builder.Build().RunAsync();
