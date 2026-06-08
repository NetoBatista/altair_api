#### [🇬🇧 English](README.md)

# Altair API

Altair API é um serviço que converte HTML ou URLs em arquivos PDF usando o Gotenberg.

## Funcionalidades
- Converter HTML em PDF.
- Converter URLs em PDF.
- Validações básicas e tratamento de erros.
- Configuração do endpoint do Gotenberg e opções de retry/timeout.

## Pré-requisitos
- .NET 10 SDK
- Docker (para executar o Gotenberg localmente)

## Quick start

1. Inicie o Gotenberg (Docker):
```bash
docker run -d -p 3000:3000 gotenberg/gotenberg:8
```

2. Execute a API:
```bash
dotnet run --project Altair.Api
```

URL padrão: `http://localhost:5000`  
Swagger / Scalar UI (desenvolvimento): `http://localhost:5000/scalar/v1`

## Configuração
- `GOTENBERG_CLIENT_URL` — endpoint do Gotenberg (ex.: `http://localhost:3000`)
- `ASPNETCORE_ENVIRONMENT` — ex.: `Development`

Exemplo para setar variáveis:
```bash
export GOTENBERG_CLIENT_URL="http://localhost:3000"
export ASPNETCORE_ENVIRONMENT=Development
```

## Estrutura do Projeto
- [Altair.Api/](Altair.Api/) — Camada Web (controllers, middleware, configuração)
  - [Altair.Api/Program.cs](Altair.Api/Program.cs)
  - [Altair.Api/Controllers/RenderController.cs](Altair.Api/Controllers/RenderController.cs)
  - [Altair.Api/Middleware/DefaultMiddleware.cs](Altair.Api/Middleware/DefaultMiddleware.cs)
- [Altair.Application/](Altair.Application/) — Lógica de negócio / UseCase
  - [Altair.Application/UseCase/RenderUseCase.cs](Altair.Application/UseCase/RenderUseCase.cs)
  - [Altair.Application/DependencyInjection.cs](Altair.Application/DependencyInjection.cs)
- [Altair.Domain/](Altair.Domain/) — Contratos, constantes, exceções
  - [Altair.Domain/Contract/HtmlToPdfRequest.cs](Altair.Domain/Contract/HtmlToPdfRequest.cs)
  - [Altair.Domain/Constant/RenderTypeConstant.cs](Altair.Domain/Constant/RenderTypeConstant.cs)
  - [Altair.Domain/Abstraction/UseCaseException.cs](Altair.Domain/Abstraction/UseCaseException.cs)

## Funcionamento (visão geral)
- A API expõe `POST /render` que recebe JSON com `type` e `content`.
- O controller delega para `IRenderUseCase`.
- `RenderUseCase` chama o cliente Gotenberg para gerar PDF.
- `DefaultMiddleware` faz logging, medição de tempo e tratamento central de erros.
- `UseCaseException` vira HTTP 400; outras falhas viram HTTP 500.

## API: POST /render
- Requisição JSON:
```json
{
  "type": "html", // ou "url"
  "content": "<h1>Olá</h1>" // HTML ou URL quando type="url"
}
```

- Exemplo (HTML):
```bash
curl -X POST http://localhost:5000/render \
  -H "Content-Type: application/json" \
  -d '{"type":"html","content":"<h1>Olá Mundo</h1>"}' --output exemplo.pdf
```

- Resposta de sucesso: HTTP 200 com `application/pdf`.
- Erro: JSON com `Status`, `Title`, `Error`, `TraceId`.

## Validações
- `HtmlToPdfRequest` exige `content` e `type`.
- `type` aceita apenas `"html"` ou `"url"`.
- Validação de URL (HTTP/HTTPS) é feita no UseCase para `type = "url"`.

## Pacotes & Notas técnicas
- Target: `net10.0`
- Pacotes principais:
  - `Gotenberg.Sharp.API.Client`
  - `Microsoft.AspNetCore.OpenApi`
  - `Scalar.AspNetCore`
- DI: `Altair.Application/DependencyInjection.cs` registra `IRenderUseCase` → `RenderUseCase`.
- Timeout: 15s; Retry: 1 (com backoff) no cliente Gotenberg.

## Tratamento de Erros & Middleware
- `DefaultMiddleware` faz log e tempo da requisição.
- `UseCaseException` -> HTTP 400.
- Exceções não tratadas -> HTTP 500 com payload JSON.

## Desenvolvimento & Build
```bash
dotnet build
dotnet run --project Altair.Api
```

## Limitações conhecidas & recomendações
- Sem autenticação/autorização.
- Não há endpoint de upload de arquivo (apenas `content` via JSON).
- Recomenda-se adicionar testes de integração contra Gotenberg local.
- Recomenda-se adicionar `docker-compose.yml` ou `.env.example` para facilitar setup local.

## Referências
- Gotenberg: https://gotenberg.dev/
- GotenbergSharpApiClient: https://changemakerstudios.github.io/GotenbergSharpApiClient/

## Licença
A definir
