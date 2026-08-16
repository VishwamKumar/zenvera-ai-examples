using ILoggerFactory loggerFactory = LoggerFactory.Create(builder =>
    builder.AddConsole().SetMinimumLevel(LogLevel.Warning));

var configuration = ExampleConfiguration.Load("zenvera-ai-examples-agent-console");
using var clients = AiClientFactory.Create(configuration, loggerFactory);

Console.WriteLine($"Support assistant ready ({clients.Provider}). Ask about orders, inventory, or weather. Type 'exit' to quit.");
Console.WriteLine();

if (clients.Provider == AiProvider.Mock)
{
    await RunMockLoopAsync();
}
else
{
    await RunAgentLoopAsync(clients.ChatClient, useDeterministicToolRouting: clients.Provider is AiProvider.Ollama);
}

static async Task RunAgentLoopAsync(IChatClient chatClient, bool useDeterministicToolRouting)
{
    var getOrderStatusTool = AIFunctionFactory.Create(
        GetOrderStatus,
        name: "GetOrderStatus",
        description: "Get the current status of a customer order by order ID.");
    var getInventoryTool = AIFunctionFactory.Create(
        GetInventory,
        name: "GetInventory",
        description: "Get inventory availability for a product name.");
    var getCurrentWeatherTool = AIFunctionFactory.Create(
        GetCurrentWeather,
        name: "GetCurrentWeather",
        description: "Get simulated current weather for a city.");

    AIAgent agent = chatClient.AsAIAgent(
        name: "OrdersAssistant",
        instructions: """
            You are a support assistant for order status, product inventory, and simulated weather.
            Your only tools are GetOrderStatus, GetInventory, and GetCurrentWeather.
            Never invent, describe, or call any other tool. Never output function-call JSON as text.
            For greetings, respond briefly and explain what you can help with.
            For unsupported requests, politely state your supported capabilities.
            If an order ID is missing, ask the user to provide one such as ORD-1001.
            """,
        tools: [getOrderStatusTool, getInventoryTool, getCurrentWeatherTool]);

    AgentSession session = await agent.CreateSessionAsync();

    while (true)
    {
        Console.Write("You: ");
        var input = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(input) || input.Equals("exit", StringComparison.OrdinalIgnoreCase))
        {
            break;
        }

        var orderId = ExtractOrderId(input);
        var requestedTool = SelectTool(input, orderId);
        if (useDeterministicToolRouting && requestedTool is not null)
        {
            // Small local models can emit or ignore tool calls inconsistently. Invoke the
            // selected tool directly after deterministic intent routing for Ollama.
            Console.WriteLine($"Agent: {InvokeSelectedTool(requestedTool, input, orderId)}");
            Console.WriteLine();
            continue;
        }

        var runOptions = new ChatClientAgentRunOptions(new ChatOptions
        {
            ToolMode = requestedTool is null
                ? ChatToolMode.None
                : ChatToolMode.RequireSpecific(requestedTool)
        });

        var response = await agent.RunAsync(input, session, runOptions);
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

[Description("Get inventory availability for a product")]
static string GetInventory(
    [Description("The product name, for example Ridgewalker Trail Boot")] string productName)
    => productName.ToLowerInvariant() switch
    {
        var name when name.Contains("ridgewalker") => "Ridgewalker Trail Boot: 14 in stock at the East warehouse.",
        var name when name.Contains("trail running") => "Trail Running Shoes: 3 in stock at the West warehouse.",
        var name when name.Contains("daypack") => "Daypack: 22 in stock at the East warehouse.",
        _ => $"No inventory record found for '{productName}'."
    };

[Description("Get simulated current weather for a city")]
static string GetCurrentWeather(
    [Description("The city name, for example New York")] string city)
    => city.ToLowerInvariant() switch
    {
        var name when name.Contains("new york") => "Simulated weather for New York: 22°C, partly cloudy, humidity 58%.",
        var name when name.Contains("seattle") => "Simulated weather for Seattle: 16°C, light rain, humidity 76%.",
        var name when name.Contains("london") => "Simulated weather for London: 18°C, overcast, humidity 68%.",
        _ => $"Simulated weather for {city}: 20°C, clear, humidity 50%."
    };

static string? SelectTool(string input, string? orderId)
{
    if (orderId is not null)
    {
        return "GetOrderStatus";
    }

    if (input.Contains("weather", StringComparison.OrdinalIgnoreCase))
    {
        return "GetCurrentWeather";
    }

    return input.Contains("inventory", StringComparison.OrdinalIgnoreCase)
        || input.Contains("in stock", StringComparison.OrdinalIgnoreCase)
        || input.Contains("stock", StringComparison.OrdinalIgnoreCase)
            ? "GetInventory"
            : null;
}

static string InvokeSelectedTool(string toolName, string input, string? orderId)
    => toolName switch
    {
        "GetOrderStatus" => GetOrderStatus(orderId!),
        "GetCurrentWeather" => GetCurrentWeather(ExtractValueAfter(input, " in ") ?? "the requested city"),
        "GetInventory" => GetInventory(ExtractKnownProduct(input) ?? input),
        _ => "That tool is not supported."
    };

static string? ExtractValueAfter(string input, string separator)
{
    var index = input.LastIndexOf(separator, StringComparison.OrdinalIgnoreCase);
    return index < 0 ? null : input[(index + separator.Length)..].Trim().TrimEnd('.', ',', '?');
}

static string? ExtractKnownProduct(string input)
{
    string[] products = ["Ridgewalker Trail Boot", "Trail Running Shoes", "Daypack"];
    return products.FirstOrDefault(product => input.Contains(product, StringComparison.OrdinalIgnoreCase));
}

static string? ExtractOrderId(string input)
{
    var token = input.Split(' ', StringSplitOptions.RemoveEmptyEntries)
        .FirstOrDefault(part => part.StartsWith("ORD-", StringComparison.OrdinalIgnoreCase));
    return token?.TrimEnd('.', ',', '?');
}
