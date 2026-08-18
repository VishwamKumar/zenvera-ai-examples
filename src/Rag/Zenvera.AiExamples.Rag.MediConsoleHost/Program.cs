using ILoggerFactory loggerFactory = LoggerFactory.Create(builder =>
    builder.AddSimpleConsole().SetMinimumLevel(LogLevel.Information));

var configuration = ExampleConfiguration.Load("zenvera-ai-examples-rag-medi");
using var clients = AiClientFactory.Create(configuration, loggerFactory);

IngestionDocumentReader reader = new MarkdownReader();
var chunkerOptions = new IngestionChunkerOptions(TiktokenTokenizer.CreateForEncoding("o200k_base"))
{
    MaxTokensPerChunk = 1200,
    OverlapTokens = 150
};
IngestionChunker<string> chunker = new SemanticSimilarityChunker(clients.EmbeddingGenerator, chunkerOptions);

var databasePath = Path.Combine(AppContext.BaseDirectory, "medi-vectors.db");
using SqliteVectorStore vectorStore = new(
    $"Data Source={databasePath};Pooling=false",
    new() { EmbeddingGenerator = clients.EmbeddingGenerator });

using VectorStoreWriter<string> writer = new(
    vectorStore,
    dimensionCount: clients.EmbeddingDimensions,
    new VectorStoreWriterOptions { CollectionName = "product-docs" });

using IngestionPipeline<string> pipeline = new(reader, chunker, writer, loggerFactory: loggerFactory);

var docs = new DirectoryInfo(Path.Combine(AppContext.BaseDirectory, "sample-docs"));
var ingestedAnything = false;

await foreach (var result in pipeline.ProcessAsync(docs, searchPattern: "*.md"))
{
    Console.WriteLine($"Ingested '{result.DocumentId}'. Succeeded: {result.Succeeded}");
    if (result.Succeeded)
    {
        ingestedAnything = true;
    }
    else if (result.Exception is not null)
    {
        Console.WriteLine($"  {result.Exception.Message}");
    }
}

if (!ingestedAnything)
{
    Console.WriteLine("No documents were ingested.");
    return;
}

var collection = writer.VectorStoreCollection;
var history = new List<ChatMessage>();
Console.WriteLine($"MEDI RAG ready ({clients.Provider}). Ask about the Ridgewalker Trail Boot.");
Console.WriteLine();

while (true)
{
    Console.Write("You: ");
    var input = Console.ReadLine();
    if (string.IsNullOrWhiteSpace(input) || input.Equals("exit", StringComparison.OrdinalIgnoreCase))
    {
        break;
    }

    var contexts = new List<string>();
    await foreach (var result in collection.SearchAsync(input, top: 3))
    {
        if (result.Record.TryGetValue("content", out var content) && content is string text)
        {
            contexts.Add(text);
        }
    }

    var context = string.Join("\n\n---\n\n", contexts);
    List<ChatMessage> messages =
    [
        new(ChatRole.System,
            "Answer using ONLY the context below. If the answer is not in context, say you don't know.\n\n" +
            $"Context:\n{context}"),
        .. history,
        new(ChatRole.User, input)
    ];

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
