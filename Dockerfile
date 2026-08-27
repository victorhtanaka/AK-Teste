# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copia arquivos csproj e restaura dependências
COPY ["WeatherDashboard.sln", "./"]
COPY ["src/WeatherDashboard.Domain/WeatherDashboard.Domain.csproj", "src/WeatherDashboard.Domain/"]
COPY ["src/WeatherDashboard.Application/WeatherDashboard.Application.csproj", "src/WeatherDashboard.Application/"]
COPY ["src/WeatherDashboard.Infrastructure/WeatherDashboard.Infrastructure.csproj", "src/WeatherDashboard.Infrastructure/"]
COPY ["src/WeatherDashboard.Api/WeatherDashboard.Api.csproj", "src/WeatherDashboard.Api/"]
COPY ["src/WeatherDashboard.Web/WeatherDashboard.Web.csproj", "src/WeatherDashboard.Web/"]
COPY ["tests/WeatherDashboard.Domain.Tests/WeatherDashboard.Domain.Tests.csproj", "tests/WeatherDashboard.Domain.Tests/"]
COPY ["tests/WeatherDashboard.Application.Tests/WeatherDashboard.Application.Tests.csproj", "tests/WeatherDashboard.Application.Tests/"]
COPY ["tests/WeatherDashboard.Api.IntegrationTests/WeatherDashboard.Api.IntegrationTests.csproj", "tests/WeatherDashboard.Api.IntegrationTests/"]
COPY ["tests/WeatherDashboard.Web.Tests/WeatherDashboard.Web.Tests.csproj", "tests/WeatherDashboard.Web.Tests/"]

RUN dotnet restore

# Copia todo o código fonte
COPY . .

# Executa os testes automatizados durante o build da imagem
RUN dotnet test --no-restore --verbosity normal

# Publica a API e o Blazor WASM
WORKDIR "/src/src/WeatherDashboard.Api"
RUN dotnet publish "WeatherDashboard.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime Stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

ENV ASPNETCORE_HTTP_PORTS=8080
ENV ASPNETCORE_ENVIRONMENT=Production

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "WeatherDashboard.Api.dll"]
