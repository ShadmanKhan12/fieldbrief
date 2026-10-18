# Development

## Ports

| Service | Port |
|---------|------|
| API | 5081 |
| Angular (`ng serve`) | 4200 |
| Postgres (compose) | 5433 → 5432 |
| Web (compose nginx) | 4201 → 80 |

## Database

```bash
docker compose up -d postgres
dotnet ef database update --project src/Fieldbrief.Infrastructure --startup-project src/Fieldbrief.Api
```

Migrations live under `src/Fieldbrief.Infrastructure/Persistence/Migrations`. Demo seed runs on API startup when no engagements exist.

## Frontend proxy

`src/web/proxy.conf.json` forwards `/api` and `/health` to `http://localhost:5081`.
