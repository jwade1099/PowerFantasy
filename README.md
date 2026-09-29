# PowerFantasy

This website will allow league mates in Sleeper fantasy football to vote on power rankings. It gives information about your league. Supplies data to the commissioner for weekly rankings. More to come.

## Tech Stack

- .NET 10 / .NET Aspire
- Blazor Server (PowerFantasy.Web)
- ASP.NET Core Minimal API (PowerFantasy.ApiService)
- PostgreSQL (via EF Core)
- Redis (Sleeper API response caching)
- [Sleeper API](https://docs.sleeper.com/)

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/) (for local Postgres/Redis containers)

## Running locally

```bash
dotnet run --project PowerFantasy.AppHost
```

This launches the Aspire AppHost, which starts Postgres, Redis, the API service, and the web frontend, and opens the Aspire dashboard.

## Solution structure

- `PowerFantasy.AppHost` — Aspire orchestrator (Postgres, Redis, service wiring)
- `PowerFantasy.ApiService` — backend API (Sleeper integration, leagues, polls, dashboard)
- `PowerFantasy.Web` — Blazor Server frontend (commissioner accounts, voting, dashboard)
- `PowerFantasy.ServiceDefaults` — shared Aspire service defaults (health checks, telemetry)
