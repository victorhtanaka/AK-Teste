# Deploy e Containerização

Esta página descreve como efetuar o empacotamento e deploy da aplicação completa utilizando Docker e Docker Compose.

---

## 1. Execução via Docker Compose

Para subir o sistema completo (API + Frontend Blazor WASM + Banco de Dados SQL Server 2022 oficial) em um único comando:

```bash
docker-compose up --build -d
```

### Serviços Inicializados:
1. **`weather-sqlserver`:** Imagem oficial `mcr.microsoft.com/mssql/server:2022-latest`, porta 1433, volume de dados persistente.
2. **`weather-app`:** Build multistage (.NET 8 SDK + ASP.NET 8 Runtime), porta 5000 exposta, aguarda health check do SQL Server.

Acesse:
- **Dashboard:** `http://localhost:5000`
- **Swagger:** `http://localhost:5000/swagger`

---

## 2. Dockerfile Multistage

O `Dockerfile` na raiz do projeto utiliza compilação em múltiplos estágios para reduzir o tamanho final da imagem:
- **Stage 1 (Build):** Imagem `mcr.microsoft.com/dotnet/sdk:8.0` para compilar e publicar com otimizações de Release.
- **Stage 2 (Runtime):** Imagem leve `mcr.microsoft.com/dotnet/aspnet:8.0` (Debian chiseled/slim) sem ferramentas desnecessárias, maximizando a segurança e velocidade de inicialização.
