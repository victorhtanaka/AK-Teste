namespace WeatherDashboard.Infrastructure.Data.Repositories;

using Microsoft.EntityFrameworkCore;
using WeatherDashboard.Domain.Entities;
using WeatherDashboard.Domain.Interfaces;

public class CityRepository : ICityRepository
{
    private readonly WeatherDbContext _context;

    public CityRepository(WeatherDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<City>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Cities
            .AsNoTracking()
            .OrderBy(c => c.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<City?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Cities
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<City?> GetByNameAndStateAsync(string name, string state, CancellationToken cancellationToken = default)
    {
        return await _context.Cities
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Name.ToLower() == name.ToLower() && c.State.ToLower() == state.ToLower(), cancellationToken);
    }
}
