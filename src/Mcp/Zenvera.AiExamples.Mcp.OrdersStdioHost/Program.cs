var builder = Host.CreateApplicationBuilder(args);

builder.Logging.AddConsole(options =>
{
    options.LogToStandardErrorThreshold = LogLevel.Trace;
});

builder.Services
    .AddMcpServer()
    .WithStdioServerTransport()
    .WithTools<Zenvera.AiExamples.Mcp.OrdersStdioHost.Tools.OrderTools>();

await builder.Build().RunAsync();
