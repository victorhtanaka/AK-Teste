# Guia de Instalação e Execução

## 1. Pré-requisitos
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [SQL Server LocalDB](https://learn.microsoft.com/sql/database-engine/configure-windows/sql-server-express-localdb) ou Docker instalado.
- (Opcional) Chave de API gratuita da [OpenWeatherMap](https://openweathermap.org/api).

---

## 2. Execução Local com .NET CLI

### Passo 1: Clonar o repositório
```bash
git clone https://github.com/seu-usuario/dashboard-climatico-capitais.git
cd dashboard-climatico-capitais
```

### Passo 2: Restaurar pacotes
```bash
dotnet restore
```

### Passo 3: Configuração do Banco de Dados (Opcional)
Por padrão, a aplicação já opera com **banco de dados em memória (InMemory)** e dados históricos pré-carregados para todas as 27 capitais, permitindo execução imediata sem requerer instalação de banco de dados externo.

Caso prefira utilizar um **SQL Server Local** (LocalDB, SQL Express ou Docker), configure a connection string de forma segura usando o .NET User Secrets:
```bash
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=(localdb)\MSSQLLocalDB;Database=WeatherDashboardDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True" --project src/WeatherDashboard.Api
```

E aplique as migrações:
```bash
dotnet ef database update --project src/WeatherDashboard.Infrastructure --startup-project src/WeatherDashboard.Api
```

### Passo 4: Configurar a Chave da OpenWeatherMap (Opcional)
Você pode utilizar o User Secrets para não expor sua chave:
```bash
dotnet user-secrets set "OpenWeather:ApiKey" "SUA_CHAVE_AQUI" --project src/WeatherDashboard.Api
```
> **Nota:** Se nenhuma chave for informada, o sistema executará com o gerador meteorológico simulado inteligente integrado, permitindo teste imediato sem dependência externa.

### Passo 5: Executar a Aplicação
```bash
dotnet run --project src/WeatherDashboard.Api
```

Acesse no navegador:
- **Dashboard Web (Blazor):** `http://localhost:5000` ou `https://localhost:7150`
- **Swagger API:** `https://localhost:7150/swagger`
- **Health Check:** `https://localhost:7150/health`

---

## 3. Execução via Docker Compose

Para rodar a aplicação completa (API + Blazor WASM + SQL Server 2022) em containers isolados:

```bash
docker-compose up --build
```

Acesse:
- **Aplicação:** `http://localhost:5000`
- **Swagger:** `http://localhost:5000/swagger`
- **Health Check:** `http://localhost:5000/health`

---

## 4. Executando os Testes Automatizados

Para rodar todos os testes unitários, testes de componentes bUnit e testes de integração:

```bash
dotnet test --verbosity normal
```
