namespace WeatherDashboard.Domain.Interfaces;

public class OpenWeatherResponseModel
{
    public double TemperatureC { get; set; }
    public double FeelsLikeC { get; set; }
    public double TempMinC { get; set; }
    public double TempMaxC { get; set; }
    public int Humidity { get; set; }
    public double PressureHpa { get; set; }
    public double WindSpeedMs { get; set; }
    public string WeatherDescription { get; set; } = string.Empty;
    public string WeatherIcon { get; set; } = string.Empty;
    public DateTime TimestampUtc { get; set; }
}

public interface IOpenWeatherClient
{
    bool IsApiKeyConfigured { get; }
    Task<OpenWeatherResponseModel?> GetCurrentWeatherByCoordinatesAsync(double latitude, double longitude, CancellationToken cancellationToken = default);
    Task<(bool Success, int StatusCode, string Message)> TestConnectionAsync(CancellationToken cancellationToken = default);
}
