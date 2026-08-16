# Chat

Level 1 console host for `IChatClient`: in-memory history, streaming replies, and structured output. Provider is configuration (`Mock`, `Foundry`, `Ollama`), not a second project.

## How to run

```powershell
dotnet run --project src/Chat/Zenvera.AiExamples.Chat.ConsoleHost
```

Expect `Chat ready (Mock)` (or the provider you selected). Type a question, `summary` for a `ConversationSummary` record, or `exit`.

Switch to Ollama or Foundry with user secrets. See the [provider runbook](../../docs/runbooks/providers.md).

```powershell
dotnet user-secrets set "Ai:Provider" "Ollama" --project src/Chat/Zenvera.AiExamples.Chat.ConsoleHost
```

This host uses **native** Ollama at `http://localhost:11434/v1`. Start `ollama serve` first. Aspire ChatWeb can start its own Ollama container; this example does not.

## Try it

```text
Explain dependency injection in one sentence.
Give me three uses for embeddings.
summary
```

The first two prompts demonstrate streaming and conversation history. `summary`
requests a structured `ConversationSummary`. Type `exit` to quit.

## What it omits

No RAG, tools, persistence, or UI. Those are later examples.

## Related

Next: [Manual RAG](../Rag/README.md). Same `IChatClient` with a grounded prompt: [ChatWeb](../ChatWeb/README.md).
