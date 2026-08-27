# Arquitetura do Sistema — Dashboard Climático das Capitais Brasileiras

## 1. Visão Geral

O sistema é construído seguindo rigorosamente os padrões de **Clean Architecture** e **SOLID**, garantindo o desacoplamento absoluto do domínio de negócio em relação a frameworks de banco de dados, provedores externos de previsão meteorológica e bibliotecas de interface gráfica.

---

## 2. Diagrama de Arquitetura Lógica

![Diagrama de Arquitetura Lógica](architecture-logical.svg)

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

## 3. Diagrama de Arquitetura Física & Infraestrutura de Deploy

![Diagrama de Arquitetura Física](architecture-physical.svg)

```mermaid
flowchart LR
    User[Navegador do Usuário<br/>Desktop / Mobile] -- "HTTPS (Porta 443 / 8080)" --> DockerApp

    subgraph DockerHost["Ambiente de Execução (Docker Host / Cloud VM / Azure)"]
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

## 4. Modelagem do Banco de Dados

### 4.1 Entidade `Cities`
- `Id` (INT, PK, Identity): Identificador único da capital.
- `Name` (NVARCHAR(100), NOT NULL): Nome da capital (ex: Curitiba).
- `State` (NVARCHAR(2), NOT NULL): Sigla da UF (ex: PR).
- `Latitude` (FLOAT, NOT NULL): Latitude geográfica oficial do IBGE.
- `Longitude` (FLOAT, NOT NULL): Longitude geográfica oficial do IBGE.
- `OpenWeatherCityId` (NVARCHAR(50), NULL): ID opcional da cidade no provedor.
- *Índice Único:* `(Name, State)`.

### 4.2 Entidade `WeatherReadings`
- `Id` (BIGINT, PK, Identity): Identificador sequencial da leitura.
- `CityId` (INT, FK -> `Cities.Id`, NOT NULL): Referência à capital.
- `CollectedAtUtc` (DATETIME2, NOT NULL): Data/hora em UTC da coleta.
- `TemperatureC` (FLOAT, NOT NULL): Temperatura em graus Celsius.
- `FeelsLikeC` (FLOAT, NOT NULL): Sensação térmica em °C.
- `TempMinC` (FLOAT, NOT NULL): Temperatura mínima do instante.
- `TempMaxC` (FLOAT, NOT NULL): Temperatura máxima do instante.
- `Humidity` (INT, NOT NULL): Umidade relativa do ar (%).
- `PressureHpa` (FLOAT, NOT NULL): Pressão atmosférica em hPa.
- `WindSpeedMs` (FLOAT, NOT NULL): Velocidade do vento em m/s.
- `WeatherDescription` (NVARCHAR(150), NOT NULL): Descrição meteorológica oficial.
- `WeatherIcon` (NVARCHAR(20), NOT NULL): Código do ícone da condição.
- *Índice Composto de Alta Performance:* `(CityId, CollectedAtUtc)`.
