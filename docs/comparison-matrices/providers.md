# Providers

Application code talks to `IChatClient` and `IEmbeddingGenerator<string, Embedding<float>>`. Registration is the only thing that changes.

| `Ai:Provider` | Chat | Embeddings | Network | Notes |
|---|---|---|---|---|
| `Mock` (default) | Deterministic replies; quotes RAG context | Hash embedding (128-d) | None | CI and first run |
| `Foundry` | Azure OpenAI / Microsoft Foundry | `text-embedding-3-small` (1536-d) | Yes | `Foundry:Endpoint` + `Foundry:ApiKey`, or `ConnectionStrings:openai` |
| `Ollama` | OpenAI-compatible local server | `all-minilm` (384-d) | Local | Also used for Foundry Local; set `Ollama:Endpoint` |

ChatWeb AppHost starts Qdrant (and Ollama when `Ollama`) only when provider is not `Mock`.

How to start Ollama, set secrets, and distinguish native vs Aspire Ollama: [provider runbook](../runbooks/providers.md).
