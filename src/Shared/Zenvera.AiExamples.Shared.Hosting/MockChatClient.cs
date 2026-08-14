namespace Zenvera.AiExamples.Shared.Hosting;

/// <summary>
/// Offline chat stand-in. When the prompt contains retrieved context, the mock quotes it
/// so RAG examples stay runnable without a model.
/// </summary>
public sealed class MockChatClient : IChatClient
{
    public ChatClientMetadata Metadata { get; } = new("mock");

    public Task<ChatResponse> GetResponseAsync(
        IEnumerable<ChatMessage> messages,
        ChatOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var text = BuildReply(messages);
        var message = new ChatMessage(ChatRole.Assistant, text);
        return Task.FromResult(new ChatResponse([message]));
    }

    public async IAsyncEnumerable<ChatResponseUpdate> GetStreamingResponseAsync(
        IEnumerable<ChatMessage> messages,
        ChatOptions? options = null,
        [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var response = await GetResponseAsync(messages, options, cancellationToken).ConfigureAwait(false);
        yield return new ChatResponseUpdate
        {
            Role = ChatRole.Assistant,
            Contents = [new TextContent(response.Text)]
        };
    }

    public object? GetService(Type serviceType, object? serviceKey = null)
        => serviceType == typeof(ChatClientMetadata) ? Metadata : null;

    public void Dispose()
    {
    }

    private static string BuildReply(IEnumerable<ChatMessage> messages)
    {
        var list = messages.ToList();
        var lastUser = list.LastOrDefault(message => message.Role == ChatRole.User)?.Text ?? string.Empty;
        var system = string.Join("\n", list.Where(message => message.Role == ChatRole.System).Select(message => message.Text));

        const string marker = "Context:";
        var contextIndex = system.IndexOf(marker, StringComparison.OrdinalIgnoreCase);
        if (contextIndex >= 0)
        {
            var context = system[(contextIndex + marker.Length)..].Trim();
            if (context.Length > 600)
            {
                context = context[..600] + "…";
            }

            return $"[mock] Grounded from retrieved context:\n{context}";
        }

        if (lastUser.Equals("summary", StringComparison.OrdinalIgnoreCase))
        {
            return """{"topic":"workshop chat","sentiment":"neutral","followUpQuestions":["What should we try next?"]}""";
        }

        return $"[mock] You said: {lastUser}";
    }
}
