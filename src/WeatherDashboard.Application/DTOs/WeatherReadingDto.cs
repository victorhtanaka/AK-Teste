namespace WeatherDashboard.Application.DTOs;

public class WeatherReadingDto
{
    public long Id { get; set; }
    public int CityId { get; set; }
    public string CityName { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public DateTime CollectedAtUtc { get; set; }
    public double TemperatureC { get; set; }
    public double FeelsLikeC { get; set; }
    public double TempMinC { get; set; }
    public double TempMaxC { get; set; }
    public int Humidity { get; set; }
    public double PressureHpa { get; set; }
    public double WindSpeedMs { get; set; }
    public string WeatherDescription { get; set; } = string.Empty;
    public string WeatherIcon { get; set; } = string.Empty;
}
