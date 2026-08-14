# Zenvera AI Examples

[![.NET](https://img.shields.io/badge/.NET-10.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![License](https://img.shields.io/badge/license-MIT-green.svg)](LICENSE)

Focused .NET 10 examples for Microsoft.Extensions.AI, RAG, MCP, Microsoft Agent Framework, Aspire chat, and adding AI to an existing app.

These are learning references, not production platform components.

**Created and maintained by [Vishwa Kumar](https://vishwa.me).**

## Examples

| # | Example | Area | Type | Level | Host |
|---:|---|---|---|---|---|
| 1 | Streaming chat | [Chat](src/Chat/README.md) | Chat / `IChatClient` | 1 | Console |
| 2 | Manual in-memory RAG | [Rag](src/Rag/README.md) | RAG | 1 | Console |
| 3 | MEDI ingestion RAG | [Rag](src/Rag/README.md) | RAG / DataIngestion | 1 | Console |
| 4 | Aspire grounded chat | [ChatWeb](src/ChatWeb/README.md) | RAG web + Aspire | 2 | Blazor + AppHost |
| 5 | MCP stdio tools | [Mcp](src/Mcp/README.md) | MCP | 1 | Stdio |
| 6 | MCP order tools | [Mcp](src/Mcp/README.md) | MCP | 1 | Stdio |
| 7 | Microsoft Agent Framework | [Agent](src/Agent/README.md) | Agent | 1 | Console |
| 8 | Retrofit AI into a catalog | [Applied](src/Applied/README.md) | Applied AI | 2 | API + Blazor + AppHost |

Foundry (Microsoft Foundry / Azure OpenAI) and Ollama (or any OpenAI-compatible local endpoint) are **one project per example**, selected with `Ai:Provider`. Default is `Mock` so restore, build, and tests run without API keys.

See the [catalog](docs/catalog.md), [learning path](docs/learning-path.md), [provider matrix](docs/comparison-matrices/providers.md), and [provider runbook](docs/runbooks/providers.md).

## Prerequisites

- [.NET SDK 10](https://dotnet.microsoft.com/download/dotnet/10.0)
- Optional: Microsoft Foundry (Azure OpenAI) endpoint and key
- Optional: Ollama or Foundry Local for `Ai:Provider=Ollama`
- Docker Desktop or compatible runtime only when ChatWeb uses Foundry or Ollama (Qdrant, and Ollama container)

## Solutions

The root solution is the CI and “open everything” view. Each area also has a smaller solution so you can load one example without the rest of the repo.

| Area | Solution | Build | Test |
|---|---|---|---|
| All | `zenvera-ai-examples.slnx` | `dotnet build zenvera-ai-examples.slnx` | `dotnet test zenvera-ai-examples.slnx` |
| Chat | `solutions/zenvera.ai-examples.chat.slnx` | `dotnet build solutions/zenvera.ai-examples.chat.slnx` | `dotnet test solutions/zenvera.ai-examples.chat.slnx` |
| RAG | `solutions/zenvera.ai-examples.rag.slnx` | `dotnet build solutions/zenvera.ai-examples.rag.slnx` | `dotnet test solutions/zenvera.ai-examples.rag.slnx` |
| ChatWeb | `solutions/zenvera.ai-examples.chatweb.slnx` | `dotnet build solutions/zenvera.ai-examples.chatweb.slnx` | — |
| MCP | `solutions/zenvera.ai-examples.mcp.slnx` | `dotnet build solutions/zenvera.ai-examples.mcp.slnx` | `dotnet test solutions/zenvera.ai-examples.mcp.slnx` |
| Agent | `solutions/zenvera.ai-examples.agent.slnx` | `dotnet build solutions/zenvera.ai-examples.agent.slnx` | `dotnet test solutions/zenvera.ai-examples.agent.slnx` |
| Applied | `solutions/zenvera.ai-examples.applied.slnx` | `dotnet build solutions/zenvera.ai-examples.applied.slnx` | — |

Run commands from the repository root.

## Provider configuration

Set `Ai:Provider` to `Mock`, `Foundry`, or `Ollama` in user secrets or environment variables. Leave committed `appsettings.json` at `Mock`.

```powershell
dotnet user-secrets set "Ai:Provider" "Foundry" --project src/Chat/Zenvera.AiExamples.Chat.ConsoleHost
dotnet user-secrets set "Foundry:Endpoint" "https://YOUR-RESOURCE.openai.azure.com/" --project src/Chat/Zenvera.AiExamples.Chat.ConsoleHost
dotnet user-secrets set "Foundry:ApiKey" "YOUR-KEY" --project src/Chat/Zenvera.AiExamples.Chat.ConsoleHost
```

Aspire ChatWeb can also use `ConnectionStrings:openai` = `Endpoint=...;Key=...` on the AppHost.

Console Chat, RAG, and Agent call **native** Ollama (`ollama serve` on port 11434). ChatWeb AppHost starts an Ollama **container** when provider is `Ollama`. Connection refused on `localhost:11434` means the native server is not listening — see the [provider runbook](docs/runbooks/providers.md).

## Run

```powershell
dotnet run --project src/Chat/Zenvera.AiExamples.Chat.ConsoleHost
dotnet run --project src/Rag/Zenvera.AiExamples.Rag.ManualConsoleHost
dotnet run --project src/Rag/Zenvera.AiExamples.Rag.MediConsoleHost
dotnet run --project src/ChatWeb/Zenvera.AiExamples.ChatWeb.AppHost
dotnet run --project src/Agent/Zenvera.AiExamples.Agent.ConsoleHost
dotnet run --project src/Applied/Zenvera.AiExamples.Applied.AppHost
```

MCP stdio hosts are meant for an MCP client (VS Code / Visual Studio / Cursor), not interactive console use.

## Repository layout

```text
src/<Area>/Zenvera.AiExamples.<Area>.<Role>/
tests/<Area>/Zenvera.AiExamples.<Area>.Tests/
```

Small examples stay single-host. ChatWeb uses Application + Infrastructure + Blazor. Applied uses Domain + Application + Infrastructure + API + Blazor. Shared hosting registers `IChatClient` and embeddings from `Ai:Provider`.

## License

Licensed under the [MIT License](LICENSE).
