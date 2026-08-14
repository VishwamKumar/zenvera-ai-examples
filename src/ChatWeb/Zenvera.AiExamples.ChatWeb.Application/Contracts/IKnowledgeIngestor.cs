namespace Zenvera.AiExamples.ChatWeb.Application.Contracts;

public interface IKnowledgeIngestor
{
    Task IngestAsync(DirectoryInfo directory, CancellationToken cancellationToken = default);
}
