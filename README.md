# Pokedex API

A small REST API that returns Pokémon information, optionally rewritten in Shakespearean or Yoda-speak. Wraps [PokéAPI](https://pokeapi.co/) and the [FunTranslations API](https://funtranslations.com/api/).

## Endpoints

| Method | Path                         | Description                                                                                                                                                              |
| ------ | ---------------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------ |
| GET    | `/pokemon/{name}`            | Basic Pokémon info.                                                                                                                                                      |
| GET    | `/pokemon/translated/{name}` | Same info, with description translated. Yoda for legendary or cave-dwelling Pokémon; Shakespeare otherwise. Falls back to the original description if translation fails. |

Example:

```bash
curl http://localhost:5000/pokemon/mewtwo
```

```json
{
  "name": "mewtwo",
  "description": "It was created by a scientist after years of horrific gene splicing and DNA engineering experiments.",
  "habitat": "rare",
  "isLegendary": true
}
```

Returns `404` for unknown Pokémon.

## Run locally (.NET)

Requires the [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0).

```bash
dotnet run --project src/Pokedex.Api
```

The API listens on `http://localhost:5000`.

## Run with Docker

Requires [Docker](https://docs.docker.com/get-docker/).

```bash
docker build -t pokedex-api .
docker run --rm -p 5000:5000 pokedex-api
```

**macOS note:** port 5000 is used by the AirPlay Receiver by default. Either disable it (System Settings → General → AirDrop & Handoff → AirPlay Receiver) or map a different host port:

```bash
docker run --rm -p 5001:5000 pokedex-api
```

## Tests

Run everything (unit + integration):

```bash
dotnet test
```

Run a single suite:

```bash
dotnet test tests/Pokedex.Api.UnitTests
dotnet test tests/Pokedex.Api.IntegrationTests
```

## Project layout

```
src/
  Pokedex.Api/
    Controllers/         HTTP layer; thin, delegates to the service.
    Services/            Orchestration and translation rules.
    Clients/             Typed HttpClients for PokéAPI and FunTranslations.
    Models/              Public response DTOs.
    Configuration/       Strongly-typed options bound from appsettings.
tests/
  Pokedex.Api.UnitTests/        Mirrors the src/ layout.
  Pokedex.Api.IntegrationTests/ End-to-end tests via WebApplicationFactory.
Dockerfile               Multi-stage build using the official .NET images.
```

## What I'd do differently for production

- **Response caching.** PokéAPI species data is effectively immutable. A 24h cache (in-memory for one node, Redis/distributed for many) would cut latency and external load dramatically.
- **Resilience policies.** Wrap both clients in [Polly](https://github.com/App-vNext/Polly): retries with jitter, circuit breaker, and a bulkhead on FunTranslations (free tier = 5 req/hour).
- **Structured logging + correlation.** Serilog with datadog. Every external HTTP call logged with `traceparent`.
- **Secrets and config.** Move base URLs and any API keys out of `appsettings.json` into environment variables or a secret store (Key Vault, AWS Secrets Manager).
- **Health checks.** `/healthz` returning aggregate status, including liveness and a lightweight dependency probe.
- **WireMock-based integration tests.** Current integration tests use in-process stubs of `IPokeApiClient` and `ITranslationClient`. A production-grade variant would swap those for [WireMock.NET](https://github.com/WireMock-Net/WireMock.Net) stubs so the real `HttpClient` wiring (BaseAddress, JSON deserialization, timeouts) is exercised too.
- **OpenAPI surface.** The OpenAPI document is already generated in Development; in production it would be gated behind an internal-only route or disabled entirely.
- **Full CI/CD pipeline.** The current GitHub Actions workflow runs `dotnet test` on pull requests and pushes to `main`. Production additions: `docker build` → push to a container registry → automated deploy → multi-arch image.i
- **Observability.** OpenTelemetry traces and metrics exported to whatever backend the team uses (Datadog, Honeycomb, OTLP collector).
