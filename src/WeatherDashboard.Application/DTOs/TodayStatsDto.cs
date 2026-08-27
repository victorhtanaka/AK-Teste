namespace WeatherDashboard.Application.DTOs;

public class TodayStatsDto
{
    public int CityId { get; set; }
    public string CityName { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public DateTime DateUtc { get; set; }
    public double TempMaxC { get; set; }
    public double TempMinC { get; set; }
    public double TempAvgC { get; set; }
    public double FeelsLikeAvgC { get; set; }
    public double HumidityAvg { get; set; }
    public double PressureAvgHpa { get; set; }
    public double WindSpeedAvgMs { get; set; }
    public string DominantWeatherDescription { get; set; } = string.Empty;
    public string DominantWeatherIcon { get; set; } = string.Empty;
    public int TotalReadings { get; set; }
    public DateTime? LastUpdatedUtc { get; set; }
}
