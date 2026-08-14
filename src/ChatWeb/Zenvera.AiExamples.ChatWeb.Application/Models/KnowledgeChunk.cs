namespace Zenvera.AiExamples.ChatWeb.Application.Models;

public sealed record KnowledgeChunk(string DocumentId, string Text, float[] Vector);

public sealed record GroundedAnswer(string Text, IReadOnlyList<string> Citations);
