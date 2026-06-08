#### [🇧🇷 Português (PT-BR)](README.pt.md)

# Altair API

Altair API is a small service that converts HTML or web URLs into PDF files using Gotenberg.

## Features
- Convert raw HTML to PDF.
- Convert web URLs to PDF.
- Basic validation and error handling.
- Configurable Gotenberg endpoint and retry/timeout options.

## Prerequisites
- .NET 10 SDK
- Docker (to run Gotenberg locally)

## Quick start

1. Start Gotenberg (Docker):
```bash
docker run -d -p 3000:3000 gotenberg/gotenberg:8
```

2. Run the API:
```bash
dotnet run --project Altair.Api
```

API default URL: `http://localhost:5000`  
Swagger / Scalar UI (development): `http://localhost:5000/scalar/v1`

## Configuration
- `GOTENBERG_CLIENT_URL` — Gotenberg service endpoint (example: `http://localhost:3000`)
- `ASPNETCORE_ENVIRONMENT` — e.g., `Development`

You can set env vars in your shell:
```bash
export GOTENBERG_CLIENT_URL="http://localhost:3000"
export ASPNETCORE_ENVIRONMENT=Development
```

## Project Structure
- [Altair.Api/](Altair.Api/) — Web API (controllers, middleware, configuration)
  - [Altair.Api/Program.cs](Altair.Api/Program.cs)
  - [Altair.Api/Controllers/RenderController.cs](Altair.Api/Controllers/RenderController.cs)
  - [Altair.Api/Middleware/DefaultMiddleware.cs](Altair.Api/Middleware/DefaultMiddleware.cs)
- [Altair.Application/](Altair.Application/) — Use case / business logic
  - [Altair.Application/UseCase/RenderUseCase.cs](Altair.Application/UseCase/RenderUseCase.cs)
  - [Altair.Application/DependencyInjection.cs](Altair.Application/DependencyInjection.cs)
- [Altair.Domain/](Altair.Domain/) — Contracts, constants, exceptions
  - [Altair.Domain/Contract/HtmlToPdfRequest.cs](Altair.Domain/Contract/HtmlToPdfRequest.cs)
  - [Altair.Domain/Constant/RenderTypeConstant.cs](Altair.Domain/Constant/RenderTypeConstant.cs)
  - [Altair.Domain/Abstraction/UseCaseException.cs](Altair.Domain/Abstraction/UseCaseException.cs)

## How it works (high level)
- The API exposes `POST /render` that accepts a JSON body with `type` and `content`.
- Controller delegates to an `IRenderUseCase` implementation.
- `RenderUseCase` uses the `Gotenberg.Sharp.API.Client` to call Gotenberg:
  - For `type = "html"` → HTML to PDF
  - For `type = "url"` → URL to PDF
- `DefaultMiddleware` wraps requests for logging, timing, and central error handling.
- Business errors throw `UseCaseException` and are returned as HTTP 400; unexpected exceptions return HTTP 500.

## API: POST /render
- URL: `POST http://localhost:5000/render`
- Request JSON:
```json
{
  "type": "html", // or "url"
  "content": "<h1>Hello</h1>" // HTML string or a URL when type="url"
}
```

- Example (HTML):
```bash
curl -X POST http://localhost:5000/render \
  -H "Content-Type: application/json" \
  -d '{"type":"html","content":"<h1>Hello World</h1>"}' --output sample.pdf
```

- Example (URL):
```bash
curl -X POST http://localhost:5000/render \
  -H "Content-Type: application/json" \
  -d '{"type":"url","content":"https://example.com"}' --output sample.pdf
```

- Success: HTTP 200 with binary `application/pdf` stream.
- Error: JSON with fields `Status`, `Title`, `Error`, `TraceId`.

## Validation
- `HtmlToPdfRequest` requires `content` and `type`.
- `type` must be either `"html"` or `"url"` (regex enforced).
- URL validation for `type = "url"` is performed in the use case (accepts HTTP/HTTPS only).

## Packages & Technical Notes
- Target framework: `net10.0`
- Key NuGet packages:
  - `Gotenberg.Sharp.API.Client` (used to call Gotenberg)
  - `Microsoft.AspNetCore.OpenApi` (OpenAPI)
  - `Scalar.AspNetCore`
- DI: `Altair.Application/DependencyInjection.cs` registers `IRenderUseCase` → `RenderUseCase`.
- Retry & timeout: Gotenberg client configured with 15s timeout and 1 retry (backoff).

## Error handling & middleware
- `DefaultMiddleware` logs requests and times them.
- `UseCaseException` -> returned as HTTP 400 (business error).
- Other exceptions -> HTTP 500 with JSON error payload.

## Development & Build
```bash
dotnet build
dotnet run --project Altair.Api
```

## Known limitations & suggestions
- No authentication/authorization
- No file upload endpoint (only JSON payload with content or URL)
- No automated tests included (recommended: add integration tests against local Gotenberg)
- Consider adding a `docker-compose.yml` or `.env.example` for easier local setup

## References
- Gotenberg official: https://gotenberg.dev/
- GotenbergSharpApiClient: https://changemakerstudios.github.io/GotenbergSharpApiClient/

## License
TBD
