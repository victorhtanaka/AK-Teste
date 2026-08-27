namespace WeatherDashboard.Application.Interfaces;

using WeatherDashboard.Application.DTOs;

public interface IWeatherService
{
    Task<WeatherReadingDto?> GetCurrentWeatherAsync(int cityId, CancellationToken cancellationToken = default);
    Task<WeatherHistoryResponseDto> GetWeatherHistoryAsync(WeatherHistoryFilterDto filter, CancellationToken cancellationToken = default);
    Task<TodayStatsDto> GetTodayStatsAsync(int cityId, CancellationToken cancellationToken = default);
    Task<NationalSummaryDto> GetNationalSummaryAsync(CancellationToken cancellationToken = default);
    Task<byte[]> ExportHistoryCsvAsync(WeatherHistoryFilterDto filter, CancellationToken cancellationToken = default);
}
