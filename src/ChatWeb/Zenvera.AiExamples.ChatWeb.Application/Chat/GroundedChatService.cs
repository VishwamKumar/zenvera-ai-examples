namespace Zenvera.AiExamples.ChatWeb.Application.Chat;

public sealed class GroundedChatService(
    IChunkStore store,
    IEmbeddingGenerator<string, Embedding<float>> embeddings,
    IChatClient chatClient,
    ILogger<GroundedChatService> logger)
{
    public async Task<GroundedAnswer> AskAsync(
        string question,
        IReadOnlyList<ChatMessage> history,
        CancellationToken cancellationToken = default)
    {
        var query = await embeddings.GenerateAsync(question, cancellationToken: cancellationToken);
        var matches = await store.SearchAsync(query.Vector.ToArray(), topK: 3, cancellationToken);
        var context = string.Join("\n\n---\n\n", matches.Select(match => match.Text));

        logger.LogInformation("Retrieved {Count} chunks for grounded chat.", matches.Count);

        var messages = new List<ChatMessage>
        {
            new(ChatRole.System,
                "Answer using ONLY the context below. If the answer is not in the context, say you don't know.\n\n" +
                $"Context:\n{context}")
        };
        messages.AddRange(history);
        messages.Add(new ChatMessage(ChatRole.User, question));

        var response = await chatClient.GetResponseAsync(messages, cancellationToken: cancellationToken);
        return new GroundedAnswer(response.Text, matches.Select(match => match.DocumentId).Distinct().ToArray());
    }
}
