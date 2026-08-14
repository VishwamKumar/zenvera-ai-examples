# Agent

Level 1 console using Microsoft Agent Framework. One order-status tool (`ORD-1001`, `ORD-1002`, `ORD-1003`).

Mock invokes the tool directly so the example runs without a model that can choose tools. Foundry and Ollama wrap `IChatClient` as an `AIAgent` and let the model call `GetOrderStatus`.

## How to run

```powershell
dotnet run --project src/Agent/Zenvera.AiExamples.Agent.ConsoleHost
```

Ask `What is the status of ORD-1001?` Type `exit` to quit.

Ollama and Foundry: [provider runbook](../../docs/runbooks/providers.md). User-secret ID: `zenvera-ai-examples-agent-console`. Native Ollama, same as Chat.

## What it omits

No planning traces, no multi-agent graph, no evaluation. Those are in `zenvera.ai-labs`.

## Related

Previous: [MCP order tools](../Mcp/README.md) (same order IDs, client-hosted tools). Next: [Applied](../Applied/README.md).
