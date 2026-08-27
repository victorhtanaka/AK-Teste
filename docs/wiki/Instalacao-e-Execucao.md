# Guia de Instalação e Execução

Este guia fornece o passo a passo detalhado para executar o sistema em ambiente local de desenvolvimento.

---

## 1. Pré-requisitos

- **.NET 8 SDK** (versão 8.0 ou superior instalada): `dotnet --version`
- **Git** para clonagem do repositório
- *(Opcional)* **SQL Server 2022 / LocalDB** (caso não esteja instalado, a aplicação utiliza fallback automático para banco InMemory)
- *(Opcional)* **Docker & Docker Compose**

---

## 2. Execução Rápida em 1 Clique

### No Windows (PowerShell):
```powershell
.\scripts\run-local.ps1
```

### No Linux / macOS (Bash):
```bash
chmod +x ./scripts/run-local.sh
./scripts/run-local.sh
```

O script automaticamente restaura pacotes, executa todos os 19 testes automatizados e inicializa o servidor.

---

## 3. Execução Manual via .NET CLI

### Passo 1: Clonar o Repositório
```bash
git clone <URL_DO_REPOSITORIO>
cd AK-Teste
```

### Passo 2: Restaurar Dependências
```bash
dotnet restore
```

### Passo 3: Configurar Chave da OpenWeatherMap (Opcional)
```bash
dotnet user-secrets set "OpenWeather:ApiKey" "SUA_API_KEY_AQUI" --project src/WeatherDashboard.Api
```

### Passo 4: Executar a Aplicação
```bash
dotnet run --project src/WeatherDashboard.Api --launch-profile http
```

### Passo 5: Acessar no Navegador
- **Dashboard Climático:** [http://localhost:5158](http://localhost:5158)
- **Documentação Swagger:** [http://localhost:5158/swagger](http://localhost:5158/swagger)
- **Health Check:** [http://localhost:5158/health](http://localhost:5158/health)
