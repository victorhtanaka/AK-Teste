# Guia de Deploy

## 1. Deploy em Nuvem com Azure App Service e Azure SQL

### 1.1 Azure SQL Database
1. Crie um servidor Azure SQL e um banco de dados `WeatherDashboardDb`.
2. Configure as regras de firewall para permitir conexões de serviços Azure.
3. Obtenha a Connection String no formato:
   `Server=tcp:seu-servidor.database.windows.net,1433;Initial Catalog=WeatherDashboardDb;Persist Security Info=False;User ID=seu-usuario;Password=sua-senha;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;`

### 1.2 Azure App Service (Linux / .NET 8)
1. Crie um App Service plano B1 ou superior.
2. Nas configurações de aplicação (**Configuration** > **Application settings**), defina:
   - `ConnectionStrings__DefaultConnection`: [Sua Connection String do Azure SQL]
   - `OpenWeather__ApiKey`: [Sua chave da OpenWeatherMap]
   - `WeatherCollector__IntervalMinutes`: `15`
3. Realize o deploy via GitHub Actions ou ZIP Deploy.

---

## 2. Deploy em VM / Servidor Linux com Docker Compose

1. Instale o Docker e Docker Compose na máquina de destino.
2. Clone o repositório.
3. Crie um arquivo `.env` com a variável:
   `OPENWEATHER_API_KEY=sua_chave_aqui`
4. Execute:
   ```bash
   docker-compose up -d --build
   ```
5. Configure um proxy reverso (Nginx ou Caddy) apontando a porta 80/443 para `http://localhost:5000` com certificado SSL gratuito via Let's Encrypt.
