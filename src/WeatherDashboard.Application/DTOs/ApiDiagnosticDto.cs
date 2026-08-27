namespace WeatherDashboard.Application.DTOs;

public class CitySyncStatusDto
{
    public int CityId { get; set; }
    public string CityName { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public int? StatusCode { get; set; }
    public bool Success { get; set; }
    public string? Message { get; set; }
    public long ResponseTimeMs { get; set; }
    public DateTime AttemptedAtUtc { get; set; }
}

public class ApiDiagnosticDto
{
    public bool IsConnected { get; set; }
    public int? LastStatusCode { get; set; }
    public string? LastErrorMessage { get; set; }
    public DateTime? LastAttemptUtc { get; set; }
    public string ProviderName { get; set; } = "OpenWeatherMap API v2.5";
    public string ApiKeyStatus { get; set; } = "Configurada";
    public List<CitySyncStatusDto> CityStatuses { get; set; } = new();
}
