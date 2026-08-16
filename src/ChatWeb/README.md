# ChatWeb

Level 2 Blazor grounded chat orchestrated by Aspire. Same lesson as the RAG consoles, with a persistent topology when the provider is not Mock.

Keep this separate from [Chat](../Chat/README.md). Chat is the MEAI console loop. ChatWeb is RAG + Aspire.

## How to run

```powershell
dotnet run --project src/ChatWeb/Zenvera.AiExamples.ChatWeb.AppHost
```

Set `Ai:Provider` on **AppHost**. Mock needs no Docker: in-process store, sample docs under `wwwroot/data`.

| Provider | AppHost starts |
|---|---|
| `Mock` | Blazor only |
| `Foundry` | Qdrant; pass `ConnectionStrings:openai` |
| `Ollama` | Qdrant and an **Ollama container** (not the native tray/`ollama serve` used by Chat) |

Foundry and Ollama need Docker. Full commands: [provider runbook](../../docs/runbooks/providers.md).

## Try it

Open the `chatweb` endpoint from the Aspire dashboard, then ask:

```text
What materials are used in the Ridgewalker Trail Boot?
How often should I reapply waterproofing spray?
Summarize the return and warranty policies.
```

The answers should be grounded in the indexed sample documents. With Ollama,
wait until the model, embedding, Qdrant, and `chatweb` resources are healthy.

## What it omits

No evaluation, no multi-tenant isolation, and no production vector-store operations.

## Related

Previous: [RAG consoles](../Rag/README.md). Adding AI onto an existing API instead of a chat UI: [Applied](../Applied/README.md).
