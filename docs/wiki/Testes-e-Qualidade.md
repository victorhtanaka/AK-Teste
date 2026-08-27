# Testes Automatizados e Qualidade

O projeto conta com uma suíte de **19 testes automatizados** cobrindo todas as camadas da aplicação, organizados sob a metodologia da Pirâmide de Testes.

---

## 1. Estrutura dos Projetos de Teste

| Projeto | Tipo | Tecnologias Utilizadas | Escopo |
|---|---|---|---|
| `WeatherDashboard.Domain.Tests` | Unitário | xUnit, FluentAssertions | Validação de invariantes de domínio e entidades. |
| `WeatherDashboard.Application.Tests` | Unitário | xUnit, Moq, FluentValidation | Validação de regras de negócio, agregações estatísticas e filtros. |
| `WeatherDashboard.Web.Tests` | Componentes | bUnit, xUnit, AngleSharp | Testes de renderização de componentes Razor e gráficos SVG. |
| `WeatherDashboard.Api.IntegrationTests` | Integração | WebApplicationFactory, InMemory DB | Testes ponta a ponta dos endpoints HTTP `/api/v1/*`. |

---

## 2. Como Executar os Testes

```bash
dotnet test WeatherDashboard.sln
```

### Resultados dos Testes:
```
Aprovado! – Com falha: 0, Aprovado: 2, Total: 2  - WeatherDashboard.Domain.Tests.dll
Aprovado! – Com falha: 0, Aprovado: 5, Total: 5  - WeatherDashboard.Application.Tests.dll
Aprovado! – Com falha: 0, Aprovado: 5, Total: 5  - WeatherDashboard.Web.Tests.dll
Aprovado! – Com falha: 0, Aprovado: 7, Total: 7  - WeatherDashboard.Api.IntegrationTests.dll
```
