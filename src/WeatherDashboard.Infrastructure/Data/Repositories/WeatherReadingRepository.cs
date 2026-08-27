namespace WeatherDashboard.Infrastructure.Data.Repositories;

using Microsoft.EntityFrameworkCore;
using WeatherDashboard.Domain.Entities;
using WeatherDashboard.Domain.Interfaces;

public class WeatherReadingRepository : IWeatherReadingRepository
{
    private readonly WeatherDbContext _context;

    public WeatherReadingRepository(WeatherDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(WeatherReading reading, CancellationToken cancellationToken = default)
    {
        await _context.WeatherReadings.AddAsync(reading, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task AddRangeAsync(IEnumerable<WeatherReading> readings, CancellationToken cancellationToken = default)
    {
        await _context.WeatherReadings.AddRangeAsync(readings, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<WeatherReading?> GetLatestByCityIdAsync(int cityId, CancellationToken cancellationToken = default)
    {
        return await _context.WeatherReadings
            .AsNoTracking()
            .Where(r => r.CityId == cityId)
            .OrderByDescending(r => r.CollectedAtUtc)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<WeatherReading>> GetByCityAndDateRangeAsync(
        int cityId,
        DateTime startUtc,
        DateTime endUtc,
        CancellationToken cancellationToken = default)
    {
        return await _context.WeatherReadings
            .AsNoTracking()
            .Where(r => r.CityId == cityId && r.CollectedAtUtc >= startUtc && r.CollectedAtUtc <= endUtc)
            .OrderBy(r => r.CollectedAtUtc)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<WeatherReading>> GetTodayReadingsByCityIdAsync(
        int cityId,
        DateTime todayUtcDate,
        CancellationToken cancellationToken = default)
    {
        return await _context.WeatherReadings
            .AsNoTracking()
            .Where(r => r.CityId == cityId && r.CollectedAtUtc >= todayUtcDate)
            .OrderBy(r => r.CollectedAtUtc)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> HasReadingRecentlyAsync(
        int cityId,
        DateTime thresholdUtc,
        CancellationToken cancellationToken = default)
    {
        return await _context.WeatherReadings
            .AsNoTracking()
            .AnyAsync(r => r.CityId == cityId && r.CollectedAtUtc >= thresholdUtc, cancellationToken);
    }
}
