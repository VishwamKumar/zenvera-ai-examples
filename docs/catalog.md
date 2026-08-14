# Example catalog

Maturity levels: Level 1 is a focused pattern; Level 2 is a layered example. These are learning-only.

| # | Example | Level | Project path | Demonstrates | External dependencies | Tests |
|---|---|---|---|---|---|---|
| 1 | Streaming chat | 1 | `src/Chat/Zenvera.AiExamples.Chat.ConsoleHost` | `IChatClient`, history, streaming, structured output, logging middleware | None in Mock; Foundry or Ollama optional | Shared |
| 2 | Manual RAG | 1 | `src/Rag/Zenvera.AiExamples.Rag.ManualConsoleHost` | Chunk, embed, cosine search, grounded prompt | None in Mock | Rag |
| 3 | MEDI RAG | 1 | `src/Rag/Zenvera.AiExamples.Rag.MediConsoleHost` | `Microsoft.Extensions.DataIngestion` pipeline + SQLite vector store | Local SQLite file | Shared |
| 4 | Aspire ChatWeb | 2 | `src/ChatWeb/` | Blazor grounded chat, Aspire, Qdrant when not Mock | Docker for Foundry/Ollama | Shared |
| 5 | MCP stdio | 1 | `src/Mcp/Zenvera.AiExamples.Mcp.StdioHost` | Weather and random tools over stdio | MCP client | — |
| 6 | MCP orders | 1 | `src/Mcp/Zenvera.AiExamples.Mcp.OrdersStdioHost` | Order, customer, inventory tools | MCP client | Mcp |
| 7 | Agent Framework | 1 | `src/Agent/Zenvera.AiExamples.Agent.ConsoleHost` | `AIAgent` + tools; Mock uses the same order tool directly | None in Mock | Shared |
| 8 | Applied catalog | 2 | `src/Applied/` | Keyword search kept; semantic search + grounded discovery added | SQLite file | Shared |

Provider is never a separate example. See [providers](comparison-matrices/providers.md) and the [provider runbook](runbooks/providers.md). Area READMEs live next to each example under `src/<Area>/README.md`.
