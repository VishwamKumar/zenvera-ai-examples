# MCP

Two Level 1 stdio servers. They are not interactive consoles. Point an MCP client (Cursor, VS Code, Visual Studio) at `dotnet run --project` with stdio transport.

| Host | Tools |
|---|---|
| `StdioHost` | `GetCurrentWeather`, `GetWeatherForecast`, `GetRandomNumber` |
| `OrdersStdioHost` | `GetOrderDetails`, `SearchOrdersByCustomer`, `GetInventory` (`ORD-1001` … `ORD-1003`) |

## Client configuration

From the repository root, example Cursor / VS Code MCP server entry:

```json
{
  "mcpServers": {
    "zenvera-orders": {
      "command": "dotnet",
      "args": [
        "run",
        "--project",
        "src/Mcp/Zenvera.AiExamples.Mcp.OrdersStdioHost",
        "--no-build"
      ]
    }
  }
}
```

Build once before `--no-build`, or drop that flag. Logging goes to stderr so it does not corrupt the stdio protocol.

These hosts do not call a chat model. Provider config does not apply.

## What it omits

No HTTP MCP transport, no auth, and no publishing.

## Related

Same order IDs used by an agent loop: [Agent](../Agent/README.md).
