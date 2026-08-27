# Arquitetura Física e Lógica

Esta página detalha os diagramas de arquitetura, padrões de projeto e decisões de engenharia adotadas no sistema.

---

## 1. Diagrama de Arquitetura Lógica

![Diagrama de Arquitetura Lógica](../architecture-logical.svg)

```mermaid
flowchart TD
    subgraph Client["Camada de Apresentação (Frontend)"]
        UI[Blazor WebAssembly SPA<br/>HTML5 Semântico & CSS3 Puro]
        SVG[Gráficos Nativos SVG<br/>TemperatureChart & HumidityChart]
        ClientService[WeatherApiClient HTTP Service]
        UI --> SVG
        UI --> ClientService
    end

    subgraph ApiHost["Camada de Host & Exposição (Backend .NET 8)"]
        Controllers[API Controllers /api/v1/*<br/>CitiesController & WeatherController]
        Worker[WeatherCollectorWorker<br/>BackgroundService PeriodicTimer 15m]
    end

    subgraph AppLayer["Camada de Aplicação (Application)"]
        AppServices[WeatherService, CityService, WeatherSyncService]
        DTOs[DTOs & ViewModels]
        Validators[FluentValidation Rules]
        AppServices --> DTOs
        AppServices --> Validators
    end

    subgraph DomainLayer["Camada de Domínio (Domain)"]
        Entities[City, WeatherReading]
        RepoInterfaces[ICityRepository, IWeatherReadingRepository]
        ClientInterfaces[IOpenWeatherClient]
    end

    subgraph InfraLayer["Camada de Infraestrutura (Infrastructure)"]
        EF[WeatherDbContext & Mappings]
        Repos[CityRepository & WeatherReadingRepository]
        OwmClient[OpenWeatherMapClient + Polly Policies]
        Repos --> EF
        OwmClient --> Polly[Polly Retry Exponencial]
    end

    subgraph External["Persistência & Provedor Externo"]
        SQL[(SQL Server 2022 / LocalDB)]
        OWM_API[OpenWeatherMap API REST]
    end

    ClientService -- "HTTP / JSON REST" --> Controllers
    Controllers --> AppServices
    Worker --> AppServices
    AppServices --> RepoInterfaces
    AppServices --> ClientInterfaces
    Repos -.-> RepoInterfaces
    OwmClient -.-> ClientInterfaces
    EF --> SQL
    OwmClient --> OWM_API
```

---

## 2. Diagrama de Arquitetura Física & Deploy

![Diagrama de Arquitetura Física](../architecture-physical.svg)

```mermaid
flowchart LR
    User[Navegador do Usuário<br/>Desktop / Mobile] -- "HTTPS / Porta 5000" --> DockerApp

    subgraph DockerHost["Ambiente de Execução (Docker / Nuvem)"]
        subgraph DockerApp["Container: weather-dashboard-app"]
            Kestrel[ASP.NET Core Kestrel Host]
            WasmStatic[Assets Estáticos Blazor WASM]
            ApiCore[Web API Core & Controllers]
            Collector[Worker de Coleta a cada 15 min]
            Kestrel --> WasmStatic
            Kestrel --> ApiCore
            Kestrel --> Collector
        end

        subgraph DockerDB["Container: weather-sqlserver"]
            DB[(SQL Server 2022 Database<br/>Porta 1433)]
        end
    end

    ApiCore -- "TCP 1433 (EF Core)" --> DB
    Collector -- "TCP 1433 (EF Core)" --> DB
    Collector -- "HTTPS (Porta 443)" --> OWM[OpenWeatherMap API Externa]
```

---

## 3. Decisões de Design & Padrões

1. **Clean Architecture & Inversão de Dependências:** O núcleo de domínio (`Domain`) não possui referências a bancos de dados, frameworks ou bibliotecas de UI.
2. **Alta Performance em Consultas Históricas:** Índice composto `(CityId, CollectedAtUtc)` no banco SQL Server permite agregações e filtragens de datas em milissegundos.
3. **Resiliência e Isolamento por Cidade:** Falhas pontuais em uma capital não afetam a coleta nem a renderização das demais capitais.
