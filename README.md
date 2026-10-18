# Fieldbrief

Engagement workbench for forward-deployed engineers: capture the customer problem, map critical systems, run HTTP health checks, log field decisions, and keep the go-live checklist honest.

Built for the FDE loop where Slack threads and Notion pages lose the thread — especially when you need a single place for “what’s broken, what we decided, and what’s left.” Runs locally with Docker; no paid APIs required.

**Current scope:** engagement list/detail, seeded Northstar Logistics demo, system map with HTTP checks, decision log, checklist, field notes. Multi-engagement create UI and auth are next.

## Stack

- ASP.NET Core (.NET 10)
- Angular 22
- PostgreSQL + EF Core
- Docker Compose
- GitHub Actions

## Why this exists

FDEs live in customer environments. The work is half integration, half judgment: which system is critical, what to unblock today, what to feed back to product. Fieldbrief keeps that loop small:

1. Open an engagement with problem + desired outcome  
2. Map customer systems and define HTTP health checks  
3. Run checks from the workbench  
4. Log decisions (and mark product feedback)  
5. Drive the checklist until go-live  

## Screens

| Area | What it does |
|------|----------------|
| Engagements | Browse seeded customer workspaces and open status at a glance |
| Engagement detail | Problem brief, systems + checks, checklist, decisions, notes |

## Quick start

**Requirements:** .NET 10 SDK, Node.js 22+, Docker Desktop

```bash
# database (host port 5433 so it can sit beside other local Postgres)
docker compose up -d postgres

# api — http://localhost:5081
dotnet run --project src/Fieldbrief.Api

# ui — http://localhost:4200 (proxies /api to the API)
cd src/web
npm install
npm start
```

Seed runs on API startup (demo engagement `NORTHSTAR-01` / Northstar Logistics).

| Endpoint | Notes |
|----------|--------|
| `GET /health` | Health check |
| `GET /api/v1/engagements` | Engagement list |
| `GET /api/v1/engagements/{id}` | Full workbench payload |
| `POST /api/v1/engagements/{id}/checks/run` | Run all HTTP checks for the engagement |

## Project layout

```text
src/Fieldbrief.Api/             HTTP host, endpoints
src/Fieldbrief.Domain/          Engagement entities + check evaluator
src/Fieldbrief.Infrastructure/  EF, seed, HTTP health runner
src/web/                        Angular UI
tests/Fieldbrief.UnitTests/     Evaluator tests
```

## Tests

```bash
dotnet test Fieldbrief.slnx
cd src/web && npm test
```

## License

MIT. See [LICENSE](LICENSE).
