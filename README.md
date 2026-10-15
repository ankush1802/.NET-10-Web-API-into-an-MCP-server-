# .NET 10 MCP Server: 3 Methods AI Should Never Get

Source code for the Core Knowledge Sharing video. An ASP.NET Core Web API (.NET 10, EF Core, PostgreSQL) that is also an MCP server, so an AI assistant such as Claude Code or GitHub Copilot can read product data through **three read-only tools** and nothing else.

## What the AI can do

| Tool | What it does |
|---|---|
| `search_products` | Search products by name, at most 20 results |
| `get_product` | Get one product by id |
| `low_stock` | Products with stock below a number, lowest first, at most 20 |

Every tool uses `AsNoTracking()`, `Take(20)` and returns `ProductDto` (Id, Name, Price, Stock). `CostPrice` and `Supplier` never reach the AI. There is no delete, no update and no "run any SQL" tool, on purpose.

The normal `ProductsController` (full CRUD for your own app) is unchanged and is not exposed to the AI.

## Run it

Requirements: .NET 10 SDK and PostgreSQL.

1. Set your connection string. `appsettings.json` has a placeholder password; pass the real one through an environment variable instead of committing it:
   ```bash
   export ConnectionStrings__Default="Host=localhost;Port=5432;Database=productsdb;Username=postgres;Password=<your password>"
   ```
   (PowerShell: `$env:ConnectionStrings__Default = '...'`)
2. `dotnet run`. The database is created and seeded on first start (`EnsureCreated`, demo only). The API listens on http://localhost:5080 and the MCP endpoint is http://localhost:5080/mcp.

## Connect Claude Code

`.mcp.json` in this folder already points at the MCP endpoint. In a second terminal:

```bash
claude --tools ""
```

`--tools ""` turns off Claude Code's own shell and file tools, so the only tools it has are the three from this API. The first time, Claude asks whether to use the server from `.mcp.json`; choose to use it. Type `/mcp` to see `products-api` with 3 tools, then ask "which products are running low on stock?".

`--tools ""` needs a shell that passes an empty argument: Git Bash, cmd or PowerShell 7 (Windows PowerShell 5.1 drops it).

## Using GitHub Copilot instead

`.vscode/mcp.json` is the same server in VS Code's format (top-level key `servers`). Click **Start** above the server in that file, open Copilot Chat in **Agent** mode, and in the tools list keep only the three `products-api` tools ticked (untick the terminal and file-editing tools).

## One more layer: a read-only database user

```sql
CREATE ROLE ai_reader LOGIN PASSWORD 'change-me';
GRANT SELECT ON ALL TABLES IN SCHEMA public TO ai_reader;
```

Use this user for the connection behind the MCP tools; keep the normal user for your controllers.

## Versions

.NET 10 · ModelContextProtocol.AspNetCore 2.2.0 · Npgsql.EntityFrameworkCore.PostgreSQL 10.0.0

Created by Ankush Agnihotri · ankush1802@outlook.com
