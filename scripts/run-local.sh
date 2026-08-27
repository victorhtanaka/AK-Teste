#!/usr/bin/env bash
set -e

echo "=========================================================="
echo "  Iniciando Weather Dashboard das Capitais Brasileiras    "
echo "=========================================================="

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
REPO_ROOT="$(cd "$SCRIPT_DIR/.." && pwd)"
cd "$REPO_ROOT"

echo ""
echo "[1/3] Restaurando pacotes e dependencias NuGet..."
dotnet restore WeatherDashboard.sln

echo ""
echo "[2/3] Executando bateria completa de testes automatizados..."
dotnet test WeatherDashboard.sln --no-restore

echo ""
echo "[3/3] Iniciando o servidor ASP.NET Core & Blazor WebAssembly..."
echo "Acesse o Dashboard em: http://localhost:5158"
echo "Acesse a documentacao Swagger em: http://localhost:5158/swagger"
echo "Pressione Ctrl+C para encerrar."

dotnet run --project src/WeatherDashboard.Api --launch-profile http
