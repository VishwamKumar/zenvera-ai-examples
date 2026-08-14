using ILoggerFactory loggerFactory = LoggerFactory.Create(builder =>
    builder.AddConsole().SetMinimumLevel(LogLevel.Warning));

var configuration = ExampleConfiguration.Load("zenvera-ai-examples-agent-console");
using var clients = AiClientFactory.Create(configuration, loggerFactory);

Console.WriteLine($"Order assistant ready ({clients.Provider}). Ask about ORD-1001, ORD-1002, or ORD-1003. Type 'exit' to quit.");
Console.WriteLine();

if (clients.Provider == AiProvider.Mock)
{
    await RunMockLoopAsync();
}
else
{
    await RunAgentLoopAsync(clients.ChatClient);
}

static async Task RunAgentLoopAsync(IChatClient chatClient)
{
    AIAgent agent = chatClient.AsAIAgent(
        name: "OrdersAssistant",
        instructions: """
            You help support staff answer questions about customer orders.
            Use the GetOrderStatus tool when someone asks about a specific order.
            """,
        tools: [AIFunctionFactory.Create(GetOrderStatus)]);

    AgentSession session = await agent.CreateSessionAsync();

    while (true)
    {
        Console.Write("You: ");
        var input = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(input) || input.Equals("exit", StringComparison.OrdinalIgnoreCase))
        {
            break;
        }

        var response = await agent.RunAsync(input, session);
        Console.WriteLine($"Agent: {response.Text}");
        Console.WriteLine();
    }
}

static Task RunMockLoopAsync()
{
    while (true)
    {
        Console.Write("You: ");
        var input = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(input) || input.Equals("exit", StringComparison.OrdinalIgnoreCase))
        {
            break;
        }

        var orderId = ExtractOrderId(input);
        Console.WriteLine(orderId is null
            ? "[mock] Ask about an order ID such as ORD-1001 so the tool can run."
            : $"[mock] {GetOrderStatus(orderId)}");
        Console.WriteLine();
    }

    return Task.CompletedTask;
}

[Description("Get the current status of a customer order")]
static string GetOrderStatus(
    [Description("The order ID, e.g. ORD-1001")] string orderId)
    => orderId.ToUpperInvariant() switch
    {
        "ORD-1001" => "Shipped three days ago, arriving in three days.",
        "ORD-1002" => "Processing, expected to ship tomorrow.",
        "ORD-1003" => "Delivered last week.",
        _ => $"No order found with ID {orderId}"
    };

static string? ExtractOrderId(string input)
{
    var token = input.Split(' ', StringSplitOptions.RemoveEmptyEntries)
        .FirstOrDefault(part => part.StartsWith("ORD-", StringComparison.OrdinalIgnoreCase));
    return token?.TrimEnd('.', ',', '?');
}
