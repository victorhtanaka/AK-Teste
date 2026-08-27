namespace WeatherDashboard.Application.Interfaces;

using WeatherDashboard.Application.DTOs;

public interface IWeatherSyncService
{
    Task<SyncResultDto> SyncAllCitiesWeatherAsync(CancellationToken cancellationToken = default);
    Task<ApiDiagnosticDto> GetLatestDiagnosticsAsync(CancellationToken cancellationToken = default);
}
