namespace WeatherDashboard.Application.DTOs;

public class SyncResultDto
{
    public int TotalCities { get; set; }
    public int SuccessCount { get; set; }
    public int FailureCount { get; set; }
    public TimeSpan Duration { get; set; }
    public List<string> Errors { get; set; } = new();
}
