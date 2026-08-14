# Applied

Level 2 catalog: keyword search stays; semantic search and grounded discovery are added. The storefront is an HTTP client of the API. It does not call the model.

```text
Domain → Application → Infrastructure → RestApiHost
                                      → BlazorServerHost
AppHost orchestrates API + storefront.
```

| Route | Behavior |
|---|---|
| `GET /api/product` | All products |
| `GET /api/product/search/{term}` | Keyword |
| `GET /api/product/aisearch/{term}` | Semantic |
| `POST /api/product/discover` | Grounded answer plus matching products |

SQLite file `catalog.db` is created next to the API on first run.

## How to run

```powershell
dotnet run --project src/Applied/Zenvera.AiExamples.Applied.AppHost
```

Set `Ai:Provider` on AppHost (`zenvera-ai-examples-applied-apphost`). Mock needs no Docker. Foundry and Ollama use the same native/Foundry settings as the consoles: [provider runbook](../../docs/runbooks/providers.md).

## What it omits

No identity, no cart, no production catalog pipeline. This is a retrofit sketch, not a store platform.

## Related

Grounded chat UI instead of a catalog API: [ChatWeb](../ChatWeb/README.md).
