Write-Host "==========================================================" -ForegroundColor Cyan
Write-Host "  Iniciando Weather Dashboard das Capitais Brasileiras    " -ForegroundColor Cyan
Write-Host "==========================================================" -ForegroundColor Cyan

if ($PSScriptRoot) {
    $RepoRoot = Split-Path -Parent $PSScriptRoot
    if (Test-Path (Join-Path $RepoRoot "WeatherDashboard.sln")) {
        Set-Location $RepoRoot
    }
}

Write-Host "`n[1/3] Restaurando pacotes e dependencias NuGet..." -ForegroundColor Yellow
dotnet restore WeatherDashboard.sln
if ($LASTEXITCODE -ne 0) {
    Write-Host "Falha na restauracao dos pacotes NuGet. Abortando inicializacao." -ForegroundColor Red
    exit 1
}

Write-Host "`n[2/3] Executando bateria completa de testes automatizados..." -ForegroundColor Yellow
dotnet test WeatherDashboard.sln --no-restore
if ($LASTEXITCODE -ne 0) {
    Write-Host "Falha na suite de testes. Abortando inicializacao." -ForegroundColor Red
    exit 1
}

Write-Host "`n[3/3] Iniciando o servidor ASP.NET Core & Blazor WebAssembly..." -ForegroundColor Green
Write-Host "Acesse o Dashboard em: http://localhost:5158" -ForegroundColor Cyan
Write-Host "Acesse a documentacao Swagger em: http://localhost:5158/swagger" -ForegroundColor Cyan
Write-Host "Pressione Ctrl+C para encerrar.`n" -ForegroundColor Gray

dotnet run --project src/WeatherDashboard.Api --launch-profile http
