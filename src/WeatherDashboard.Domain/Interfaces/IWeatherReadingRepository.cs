namespace WeatherDashboard.Domain.Interfaces;

using WeatherDashboard.Domain.Entities;

public interface IWeatherReadingRepository
{
    Task AddAsync(WeatherReading reading, CancellationToken cancellationToken = default);
    Task AddRangeAsync(IEnumerable<WeatherReading> readings, CancellationToken cancellationToken = default);
    Task<WeatherReading?> GetLatestByCityIdAsync(int cityId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<WeatherReading>> GetByCityAndDateRangeAsync(int cityId, DateTime startUtc, DateTime endUtc, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<WeatherReading>> GetTodayReadingsByCityIdAsync(int cityId, DateTime todayUtcDate, CancellationToken cancellationToken = default);
    Task<bool> HasReadingRecentlyAsync(int cityId, DateTime thresholdUtc, CancellationToken cancellationToken = default);
}
