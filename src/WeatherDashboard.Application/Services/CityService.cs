namespace WeatherDashboard.Application.Services;

using WeatherDashboard.Application.DTOs;
using WeatherDashboard.Application.Interfaces;
using WeatherDashboard.Domain.Interfaces;

public class CityService : ICityService
{
    private readonly ICityRepository _cityRepository;

    public CityService(ICityRepository cityRepository)
    {
        _cityRepository = cityRepository;
    }

    public async Task<IReadOnlyList<CityDto>> GetAllCitiesAsync(CancellationToken cancellationToken = default)
    {
        var cities = await _cityRepository.GetAllAsync(cancellationToken);
        return cities.Select(c => new CityDto
        {
            Id = c.Id,
            Name = c.Name,
            State = c.State,
            Latitude = c.Latitude,
            Longitude = c.Longitude,
            OpenWeatherCityId = c.OpenWeatherCityId
        }).ToList();
    }

    public async Task<CityDto?> GetCityByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var city = await _cityRepository.GetByIdAsync(id, cancellationToken);
        if (city == null) return null;

        return new CityDto
        {
            Id = city.Id,
            Name = city.Name,
            State = city.State,
            Latitude = city.Latitude,
            Longitude = city.Longitude,
            OpenWeatherCityId = city.OpenWeatherCityId
        };
    }
}
