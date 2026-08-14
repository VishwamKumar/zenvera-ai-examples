using ILoggerFactory loggerFactory = LoggerFactory.Create(builder =>
    builder.AddConsole().SetMinimumLevel(LogLevel.Warning));

var configuration = ExampleConfiguration.Load("zenvera-ai-examples-chat-console");
using var clients = AiClientFactory.Create(configuration, loggerFactory);

var history = new List<ChatMessage>
{
    new(ChatRole.System, "You are a concise assistant for .NET AI examples.")
};

Console.WriteLine($"Chat ready ({clients.Provider}). Type a message, 'summary' for structured output, or 'exit'.");
Console.WriteLine();

while (true)
{
    Console.Write("You: ");
    var input = Console.ReadLine();
    if (string.IsNullOrWhiteSpace(input) || input.Equals("exit", StringComparison.OrdinalIgnoreCase))
    {
        break;
    }

    if (input.Equals("summary", StringComparison.OrdinalIgnoreCase))
    {
        List<ChatMessage> summaryPrompt =
        [
            .. history,
            new ChatMessage(ChatRole.User, "Summarize this conversation so far.")
        ];

        var structured = await clients.ChatClient.GetResponseAsync<ConversationSummary>(summaryPrompt);
        if (structured.TryGetResult(out ConversationSummary? summary) && summary is not null)
        {
            Console.WriteLine($"Topic:      {summary.Topic}");
            Console.WriteLine($"Sentiment:  {summary.Sentiment}");
            Console.WriteLine($"Follow-ups: {string.Join(", ", summary.FollowUpQuestions)}");
        }
        else
        {
            Console.WriteLine(structured.Text);
        }

        Console.WriteLine();
        continue;
    }

    history.Add(new ChatMessage(ChatRole.User, input));
    Console.Write("Assistant: ");
    var assistantText = new System.Text.StringBuilder();

    await foreach (var update in clients.ChatClient.GetStreamingResponseAsync(history))
    {
        Console.Write(update.Text);
        assistantText.Append(update.Text);
    }

    Console.WriteLine();
    Console.WriteLine();
    history.Add(new ChatMessage(ChatRole.Assistant, assistantText.ToString()));
}

Console.WriteLine("Goodbye.");

internal sealed record ConversationSummary(
    string Topic,
    string Sentiment,
    string[] FollowUpQuestions);
