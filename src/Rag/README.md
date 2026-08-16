# RAG

Two Level 1 consoles on the same Ridgewalker Trail Boot sample. Both embed, retrieve top chunks, and stream a grounded answer. Provider is `Ai:Provider` on each host.

| Host | Retrieval | Storage |
|---|---|---|
| `ManualConsoleHost` | Split on blank lines, cosine search | In-memory |
| `MediConsoleHost` | `Microsoft.Extensions.DataIngestion` (markdown reader, semantic chunker) | Local SQLite `medi-vectors.db` |

## How to run

```powershell
dotnet run --project src/Rag/Zenvera.AiExamples.Rag.ManualConsoleHost
dotnet run --project src/Rag/Zenvera.AiExamples.Rag.MediConsoleHost
```

Ask about the boot (materials, warranty, care). Type `exit` to quit. MEDI writes `medi-vectors.db` next to the built binaries; it is local scratch, not source.

Ollama and Foundry: [provider runbook](../../docs/runbooks/providers.md). User-secret IDs are `zenvera-ai-examples-rag-manual` and `zenvera-ai-examples-rag-medi`.

## Try it

Use the same questions with both hosts and compare retrieval behavior:

```text
What is the boot upper made of?
How should I clean and dry the boots?
What does the warranty cover?
Can I return worn boots?
```

Answers are grounded only in the Ridgewalker Trail Boot sample document. The
manual retriever can be more sensitive to wording than the MEDI retriever.

## What it omits

No Aspire, no Qdrant, no citations UI. MEDI is still a console loop, not a production ingestion service.

## Related

Previous: [Chat](../Chat/README.md). Same grounded chat with a web host and optional Qdrant: [ChatWeb](../ChatWeb/README.md).
