namespace WeatherDashboard.Application.Services;

using System.Diagnostics;
using Microsoft.Extensions.Logging;
using WeatherDashboard.Application.DTOs;
using WeatherDashboard.Application.Interfaces;
using WeatherDashboard.Domain.Entities;
using WeatherDashboard.Domain.Interfaces;

public class WeatherSyncService : IWeatherSyncService
{
    private static readonly ApiDiagnosticDto _latestDiagnostic = new()
    {
        IsConnected = false,
        ProviderName = "OpenWeatherMap API v2.5",
        ApiKeyStatus = "Não verificada",
        LastAttemptUtc = null
    };

    private static readonly object _lock = new();

    private readonly ICityRepository _cityRepository;
    private readonly IWeatherReadingRepository _weatherReadingRepository;
    private readonly IOpenWeatherClient _openWeatherClient;
    private readonly ILogger<WeatherSyncService> _logger;

    public WeatherSyncService(
        ICityRepository cityRepository,
        IWeatherReadingRepository weatherReadingRepository,
        IOpenWeatherClient openWeatherClient,
        ILogger<WeatherSyncService> logger)
    {
        _cityRepository = cityRepository;
        _weatherReadingRepository = weatherReadingRepository;
        _openWeatherClient = openWeatherClient;
        _logger = logger;
    }

    public async Task<SyncResultDto> SyncAllCitiesWeatherAsync(CancellationToken cancellationToken = default)
    {
        var stopwatch = Stopwatch.StartNew();
        var cities = await _cityRepository.GetAllAsync(cancellationToken);
        var result = new SyncResultDto
        {
            TotalCities = cities.Count
        };

        var isConfigured = _openWeatherClient.IsApiKeyConfigured;
        _logger.LogInformation(
            "Iniciando sincronização climática de {Count} capitais ({Mode})...",
            cities.Count,
            isConfigured ? "OpenWeatherMap API Oficial" : "Modo Simulado Integrado");

        var newReadings = new List<WeatherReading>();
        var cityStatuses = new List<CitySyncStatusDto>();

        foreach (var city in cities)
        {
            if (cancellationToken.IsCancellationRequested) break;

            var cityStopwatch = Stopwatch.StartNew();
            try
            {
                var weatherData = await _openWeatherClient.GetCurrentWeatherByCoordinatesAsync(
                    city.Latitude,
                    city.Longitude,
                    cancellationToken);

                cityStopwatch.Stop();

                if (weatherData != null)
                {
                    var reading = new WeatherReading
                    {
                        CityId = city.Id,
                        CollectedAtUtc = weatherData.TimestampUtc,
                        TemperatureC = weatherData.TemperatureC,
                        FeelsLikeC = weatherData.FeelsLikeC,
                        TempMinC = weatherData.TempMinC,
                        TempMaxC = weatherData.TempMaxC,
                        Humidity = weatherData.Humidity,
                        PressureHpa = weatherData.PressureHpa,
                        WindSpeedMs = weatherData.WindSpeedMs,
                        WeatherDescription = weatherData.WeatherDescription,
                        WeatherIcon = weatherData.WeatherIcon
                    };

                    newReadings.Add(reading);
                    result.SuccessCount++;

                    cityStatuses.Add(new CitySyncStatusDto
                    {
                        CityId = city.Id,
                        CityName = city.Name,
                        State = city.State,
                        StatusCode = 200,
                        Success = true,
                        Message = isConfigured
                            ? "Leitura recebida com sucesso da OpenWeatherMap."
                            : "Leitura gerada com sucesso pelo simulador meteorológico integrado.",
                        ResponseTimeMs = cityStopwatch.ElapsedMilliseconds,
                        AttemptedAtUtc = DateTime.UtcNow
                    });
                }
                else
                {
                    var errorMsg = $"Falha ao obter dados meteorológicos para {city.Name}-{city.State}.";
                    _logger.LogWarning("{ErrorMsg}", errorMsg);
                    result.Errors.Add(errorMsg);
                    result.FailureCount++;

                    cityStatuses.Add(new CitySyncStatusDto
                    {
                        CityId = city.Id,
                        CityName = city.Name,
                        State = city.State,
                        StatusCode = 401,
                        Success = false,
                        Message = "OpenWeatherMap rejeitou a requisição (chave aguardando ativação).",
                        ResponseTimeMs = cityStopwatch.ElapsedMilliseconds,
                        AttemptedAtUtc = DateTime.UtcNow
                    });
                }
            }
            catch (Exception ex)
            {
                cityStopwatch.Stop();
                var errorMsg = $"Erro ao coletar clima de {city.Name}-{city.State}: {ex.Message}";
                _logger.LogError(ex, "{ErrorMsg}", errorMsg);
                result.Errors.Add(errorMsg);
                result.FailureCount++;

                cityStatuses.Add(new CitySyncStatusDto
                {
                    CityId = city.Id,
                    CityName = city.Name,
                    State = city.State,
                    StatusCode = 500,
                    Success = false,
                    Message = ex.Message,
                    ResponseTimeMs = cityStopwatch.ElapsedMilliseconds,
                    AttemptedAtUtc = DateTime.UtcNow
                });
            }
        }

        if (newReadings.Count > 0)
        {
            await _weatherReadingRepository.AddRangeAsync(newReadings, cancellationToken);
        }

        stopwatch.Stop();
        result.Duration = stopwatch.Elapsed;

        // Atualiza os diagnósticos em memória
        lock (_lock)
        {
            _latestDiagnostic.ApiKeyStatus = isConfigured ? "Configurada (User Secrets)" : "Não configurada (Modo Simulado)";
            _latestDiagnostic.IsConnected = isConfigured && result.SuccessCount > 0;
            _latestDiagnostic.LastStatusCode = isConfigured
                ? (result.SuccessCount > 0 ? 200 : (result.FailureCount > 0 ? 401 : null))
                : 200;
            _latestDiagnostic.LastErrorMessage = isConfigured && result.FailureCount > 0 && result.SuccessCount == 0 
                ? "A chave de API da OpenWeatherMap retornou 401 (aguardando ativação/propagação na conta)."
                : null;
            _latestDiagnostic.LastAttemptUtc = DateTime.UtcNow;
            _latestDiagnostic.CityStatuses = cityStatuses;
        }

        _logger.LogInformation(
            "Ciclo de sincronização concluído em {Duration}ms. Sucesso: {Success}/{Total}, Falhas: {Failures}",
            stopwatch.ElapsedMilliseconds,
            result.SuccessCount,
            result.TotalCities,
            result.FailureCount);

        return result;
    }

    public async Task<ApiDiagnosticDto> GetLatestDiagnosticsAsync(CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            var isConfigured = _openWeatherClient.IsApiKeyConfigured;
            if (_latestDiagnostic.LastAttemptUtc == null)
            {
                _latestDiagnostic.ApiKeyStatus = isConfigured ? "Configurada (User Secrets)" : "Não configurada (Modo Simulado)";
                if (isConfigured)
                {
                    var test = _openWeatherClient.TestConnectionAsync(cancellationToken).GetAwaiter().GetResult();
                    _latestDiagnostic.IsConnected = test.Success;
                    _latestDiagnostic.LastStatusCode = test.StatusCode;
                    _latestDiagnostic.LastErrorMessage = test.Success ? null : test.Message;
                }
                else
                {
                    _latestDiagnostic.IsConnected = false;
                    _latestDiagnostic.LastStatusCode = 200;
                    _latestDiagnostic.LastErrorMessage = null;
                }
                _latestDiagnostic.LastAttemptUtc = DateTime.UtcNow;
            }

            return Task.FromResult(new ApiDiagnosticDto
            {
                IsConnected = _latestDiagnostic.IsConnected,
                LastStatusCode = _latestDiagnostic.LastStatusCode,
                LastErrorMessage = _latestDiagnostic.LastErrorMessage,
                LastAttemptUtc = _latestDiagnostic.LastAttemptUtc,
                ProviderName = _latestDiagnostic.ProviderName,
                ApiKeyStatus = _latestDiagnostic.ApiKeyStatus,
                CityStatuses = new List<CitySyncStatusDto>(_latestDiagnostic.CityStatuses)
            }).Result;
        }
    }
}
