using ILoggerFactory loggerFactory = LoggerFactory.Create(builder =>
    builder.AddConsole().SetMinimumLevel(LogLevel.Warning));

var configuration = ExampleConfiguration.Load("zenvera-ai-examples-rag-manual");
using var clients = AiClientFactory.Create(configuration, loggerFactory);

var documentPath = Path.Combine(AppContext.BaseDirectory, "sample-docs", "ridgewalker-trail-boot.md");
var document = await File.ReadAllTextAsync(documentPath);
var chunks = document
    .ReplaceLineEndings("\n")
    .Split("\n\n", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
    .Where(chunk => chunk.Length > 0)
    .ToArray();

Console.WriteLine($"Embedding {chunks.Length} chunks ({clients.Provider})...");
var embeddings = await clients.EmbeddingGenerator.GenerateAsync(chunks);

var store = new InMemoryChunkStore();
for (var i = 0; i < chunks.Length; i++)
{
    store.Add(chunks[i], embeddings[i].Vector.ToArray());
}

var history = new List<ChatMessage>();
Console.WriteLine("Ask about the Ridgewalker Trail Boot. Type 'exit' to quit.");
Console.WriteLine();

while (true)
{
    Console.Write("You: ");
    var input = Console.ReadLine();
    if (string.IsNullOrWhiteSpace(input) || input.Equals("exit", StringComparison.OrdinalIgnoreCase))
    {
        break;
    }

    var question = await clients.EmbeddingGenerator.GenerateAsync(input);
    var topChunks = store.Search(question.Vector, topK: 3);
    var context = string.Join("\n\n---\n\n", topChunks);
    var systemPrompt = new ChatMessage(
        ChatRole.System,
        "You are a product support assistant. Answer using ONLY the context below. " +
        "If the answer is not in the context, say you don't know.\n\n" +
        $"Context:\n{context}");

    var messages = new List<ChatMessage> { systemPrompt };
    messages.AddRange(history);
    messages.Add(new ChatMessage(ChatRole.User, input));

    Console.Write("Assistant: ");
    var answer = new System.Text.StringBuilder();
    await foreach (var update in clients.ChatClient.GetStreamingResponseAsync(messages))
    {
        Console.Write(update.Text);
        answer.Append(update.Text);
    }

    Console.WriteLine();
    Console.WriteLine();
    history.Add(new ChatMessage(ChatRole.User, input));
    history.Add(new ChatMessage(ChatRole.Assistant, answer.ToString()));
}

Console.WriteLine("Goodbye.");
