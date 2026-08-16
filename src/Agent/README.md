# Agent

Level 1 console using Microsoft Agent Framework. It demonstrates order-status,
product-inventory, and simulated-weather tools.

Mock invokes the order tool directly so the example runs without a model that can
choose tools. Foundry wraps `IChatClient` as an `AIAgent` and lets the model select
tools. Ollama uses deterministic intent routing because small local models can emit
or ignore tool calls inconsistently, while greetings and unsupported prompts still
go through the agent.

## How to run

```powershell
dotnet run --project src/Agent/Zenvera.AiExamples.Agent.ConsoleHost
```

Ollama and Foundry: [provider runbook](../../docs/runbooks/providers.md). User-secret ID: `zenvera-ai-examples-agent-console`. Native Ollama, same as Chat.

## Try it

```text
What is the status of ORD-1001?
What is the status of ORD-1002?
Check order ORD-9999.
Is the Ridgewalker Trail Boot in stock?
How many Daypacks are in inventory?
What is the weather in New York?
How are you?
```

The final prompt demonstrates a normal tool-free response. Weather data is
simulated. Type `exit` to quit.

## What it omits

No planning traces, no multi-agent graph, and no evaluation harness.

## Related

Previous: [MCP order tools](../Mcp/README.md) (same order IDs, client-hosted tools). Next: [Applied](../Applied/README.md).
