namespace WeatherDashboard.Application.Interfaces;

using WeatherDashboard.Application.DTOs;

public interface ICityService
{
    Task<IReadOnlyList<CityDto>> GetAllCitiesAsync(CancellationToken cancellationToken = default);
    Task<CityDto?> GetCityByIdAsync(int id, CancellationToken cancellationToken = default);
}
