namespace WeatherDashboard.Application.DTOs;

public class NationalSummaryDto
{
    public CityTempSummaryDto? WarmestCity { get; set; }
    public CityTempSummaryDto? ColdestCity { get; set; }
    public double NationalAvgTempC { get; set; }
    public double NationalAvgHumidity { get; set; }
    public int TotalMonitoredCapitals { get; set; }
    public int TotalReadingsToday { get; set; }
    public DateTime CalculatedAtUtc { get; set; }
}

public class CityTempSummaryDto
{
    public int CityId { get; set; }
    public string CityName { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public double TemperatureC { get; set; }
    public string WeatherDescription { get; set; } = string.Empty;
    public string WeatherIcon { get; set; } = string.Empty;
}
