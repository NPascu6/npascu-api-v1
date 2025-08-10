# SwissTax API (minimal)

This repository contains a minimal Swiss finance helpers API built with ASP.NET Core 8 minimal APIs. It provides:

- `/v1/deductions/estimate` – basic tax estimation for a canton/year
- `/v1/allowances/{canton}/{year}` – returns deduction allowances
- `/health` – health check

Projects:
- `src/Api` – HTTP API
- `src/Domain` – domain entities and services
- `src/Infrastructure` – EF Core database context and migrations
- `src/Worker` – background worker skeleton
- `tests/ApiTests` – integration tests using `WebApplicationFactory`

## Development

```bash
dotnet restore SwissTax.sln
dotnet build SwissTax.sln
dotnet test
```

## Docker

Build and run locally:

```bash
docker build -t swisstax .
docker run -p 10000:10000 swisstax
```

## Render

The `render.yaml` defines services for deploying the API and worker along with managed Postgres and Redis instances.
