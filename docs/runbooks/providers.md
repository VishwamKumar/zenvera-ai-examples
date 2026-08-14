# Provider runbook

Every example talks to `IChatClient` and embeddings. Switch backends with `Ai:Provider`. Do not commit a provider other than `Mock`.

| Value | When to use | Extra requirements |
|---|---|---|
| `Mock` (default) | First run, CI, no network | None |
| `Foundry` | Azure OpenAI / Microsoft Foundry | Endpoint + API key |
| `Ollama` | Local OpenAI-compatible server (Ollama or Foundry Local) | A running server and pulled models |

Details of models and dimensions are in the [provider matrix](../comparison-matrices/providers.md).

## Console hosts (Chat, RAG, Agent)

Set the provider on the project, not globally.

```powershell
dotnet user-secrets set "Ai:Provider" "Ollama" --project src/Chat/Zenvera.AiExamples.Chat.ConsoleHost
```

Foundry:

```powershell
dotnet user-secrets set "Ai:Provider" "Foundry" --project src/Chat/Zenvera.AiExamples.Chat.ConsoleHost
dotnet user-secrets set "Foundry:Endpoint" "https://YOUR-RESOURCE.openai.azure.com/" --project src/Chat/Zenvera.AiExamples.Chat.ConsoleHost
dotnet user-secrets set "Foundry:ApiKey" "YOUR-KEY" --project src/Chat/Zenvera.AiExamples.Chat.ConsoleHost
```

User-secret IDs match the project: `zenvera-ai-examples-chat-console`, `zenvera-ai-examples-rag-manual`, `zenvera-ai-examples-rag-medi`, `zenvera-ai-examples-agent-console`.

These hosts call **native** Ollama on the machine. They do not start a container.

## Native Ollama

1. Install [Ollama](https://ollama.com/download).
2. Start the server (`ollama serve`, or the tray app) and confirm it listens on port 11434.
3. Pull the models the examples request:

```powershell
ollama pull llama3.2
ollama pull all-minilm
ollama list
```

4. Probe the OpenAI-compatible API the shared factory uses:

```powershell
Invoke-WebRequest http://127.0.0.1:11434/v1/models
```

Default config is `Ollama:Endpoint` = `http://localhost:11434/v1` and `Ollama:ChatModel` = `llama3.2`. The factory appends `/v1` if it is missing.

### Connection refused

The example URL is correct. Connection refused means nothing is accepting TCP on 11434.

- The Windows tray (`ollama app.exe`) can stay up after a failed update while `ollama.exe` is not listening. Quit the tray, then run `ollama serve`.
- Confirm with `netstat -ano | findstr 11434` and `ollama list`. `could not connect to a running Ollama instance` is the same failure.
- If `http://127.0.0.1:11434/v1/models` works but `http://localhost:11434/v1/models` does not, Ollama is bound to IPv4 only. Set `Ollama:Endpoint` to `http://127.0.0.1:11434/v1`.

## Aspire ChatWeb

Set `Ai:Provider` on **AppHost** (`src/ChatWeb/Zenvera.AiExamples.ChatWeb.AppHost`), not only on the Blazor project.

| Provider | What AppHost starts |
|---|---|
| `Mock` | Blazor only; in-memory chunk store |
| `Foundry` | Qdrant + `ConnectionStrings:openai` |
| `Ollama` | Qdrant + an **Ollama container** and the configured models |

Foundry on AppHost:

```powershell
dotnet user-secrets set "Ai:Provider" "Foundry" --project src/ChatWeb/Zenvera.AiExamples.ChatWeb.AppHost
dotnet user-secrets set "ConnectionStrings:openai" "Endpoint=https://YOUR-RESOURCE.openai.azure.com/;Key=YOUR-KEY" --project src/ChatWeb/Zenvera.AiExamples.ChatWeb.AppHost
```

Foundry and Ollama need Docker Desktop (or a compatible engine) for Qdrant, and for the Ollama container when provider is `Ollama`. That container is separate from native `ollama serve` used by the console examples.

## Applied catalog

Set `Ai:Provider` on Applied AppHost (`zenvera-ai-examples-applied-apphost`) or the API (`zenvera-ai-examples-applied-api`). The storefront is an HTTP client of the API and does not talk to the model itself. Mock needs no Docker. Foundry and Ollama use the same native/Foundry settings as the console hosts.
