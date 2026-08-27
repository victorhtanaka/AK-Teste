namespace WeatherDashboard.Api.BackgroundServices;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using WeatherDashboard.Application.Interfaces;

public class WeatherCollectorWorker : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IConfiguration _configuration;
    private readonly ILogger<WeatherCollectorWorker> _logger;

    public WeatherCollectorWorker(
        IServiceScopeFactory scopeFactory,
        IConfiguration configuration,
        ILogger<WeatherCollectorWorker> logger)
    {
        _scopeFactory = scopeFactory;
        _configuration = configuration;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var intervalMinutes = _configuration.GetValue<int>("WeatherCollector:IntervalMinutes", 15);
        if (intervalMinutes <= 0) intervalMinutes = 15;

        var interval = TimeSpan.FromMinutes(intervalMinutes);
        _logger.LogInformation("WeatherCollectorWorker iniciado. Intervalo de coleta: {Minutes} minutos.", intervalMinutes);

        // Executa uma coleta inicial imediatamente na inicialização
        try
        {
            _logger.LogInformation("Executando coleta inicial de inicialização...");
            await RunSyncCycleAsync(stoppingToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro na coleta meteorológica inicial.");
        }

        using var timer = new PeriodicTimer(interval);

        while (!stoppingToken.IsCancellationRequested && await timer.WaitForNextTickAsync(stoppingToken))
        {
            try
            {
                _logger.LogInformation("Iniciando ciclo programado de coleta meteorológica...");
                await RunSyncCycleAsync(stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("WeatherCollectorWorker cancelado.");
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro inesperado durante o ciclo do WeatherCollectorWorker.");
            }
        }

        _logger.LogInformation("WeatherCollectorWorker finalizado.");
    }

    private async Task RunSyncCycleAsync(CancellationToken cancellationToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var syncService = scope.ServiceProvider.GetRequiredService<IWeatherSyncService>();
        var result = await syncService.SyncAllCitiesWeatherAsync(cancellationToken);

        _logger.LogInformation(
            "Ciclo de coleta finalizado. Sucessos: {Success}/{Total} cidades. Falhas: {Failures}. Duração: {Duration}s",
            result.SuccessCount,
            result.TotalCities,
            result.FailureCount,
            result.Duration.TotalSeconds);
    }
}
