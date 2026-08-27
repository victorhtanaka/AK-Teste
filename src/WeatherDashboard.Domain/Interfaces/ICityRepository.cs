namespace WeatherDashboard.Domain.Interfaces;

using WeatherDashboard.Domain.Entities;

public interface ICityRepository
{
    Task<IReadOnlyList<City>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<City?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<City?> GetByNameAndStateAsync(string name, string state, CancellationToken cancellationToken = default);
}
