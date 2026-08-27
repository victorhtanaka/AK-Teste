namespace WeatherDashboard.Application.DTOs;

public class WeatherHistoryFilterDto
{
    public int CityId { get; set; }
    public DateTime? StartDateUtc { get; set; }
    public DateTime? EndDateUtc { get; set; }
}

public class WeatherHistoryPointDto
{
    public DateTime TimestampUtc { get; set; }
    public double TemperatureC { get; set; }
    public double FeelsLikeC { get; set; }
    public double TempMinC { get; set; }
    public double TempMaxC { get; set; }
    public int Humidity { get; set; }
    public double WindSpeedMs { get; set; }
    public double PressureHpa { get; set; }
    public string WeatherDescription { get; set; } = string.Empty;
    public string WeatherIcon { get; set; } = string.Empty;
}

public class WeatherHistoryResponseDto
{
    public int CityId { get; set; }
    public string CityName { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public DateTime StartUtc { get; set; }
    public DateTime EndUtc { get; set; }
    public IReadOnlyList<WeatherHistoryPointDto> Points { get; set; } = Array.Empty<WeatherHistoryPointDto>();
}
