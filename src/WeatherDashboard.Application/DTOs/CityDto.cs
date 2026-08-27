namespace WeatherDashboard.Application.DTOs;

public class CityDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string? OpenWeatherCityId { get; set; }

    public string FullDisplayName => $"{Name} - {State}";
}
