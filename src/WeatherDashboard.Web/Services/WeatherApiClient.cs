namespace WeatherDashboard.Web.Services;

using System.Globalization;
using System.Net.Http.Json;
using WeatherDashboard.Application.DTOs;

public class WeatherApiClient
{
    private readonly HttpClient _httpClient;

    public WeatherApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IReadOnlyList<CityDto>> GetAllCitiesAsync()
    {
        try
        {
            var result = await _httpClient.GetFromJsonAsync<IReadOnlyList<CityDto>>("api/v1/cities");
            return result ?? Array.Empty<CityDto>();
        }
        catch
        {
            return Array.Empty<CityDto>();
        }
    }

    public async Task<WeatherReadingDto?> GetCurrentWeatherAsync(int cityId)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<WeatherReadingDto>($"api/v1/weather/current?cityId={cityId}");
        }
        catch
        {
            return null;
        }
    }

    public async Task<TodayStatsDto?> GetTodayStatsAsync(int cityId)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<TodayStatsDto>($"api/v1/weather/stats/today?cityId={cityId}");
        }
        catch
        {
            return null;
        }
    }

    public async Task<WeatherHistoryResponseDto?> GetWeatherHistoryAsync(int cityId, DateTime? startDateUtc, DateTime? endDateUtc)
    {
        try
        {
            var query = $"api/v1/weather/history?cityId={cityId}";
            if (startDateUtc.HasValue)
            {
                query += $"&startDateUtc={startDateUtc.Value:yyyy-MM-ddTHH:mm:ssZ}";
            }
            if (endDateUtc.HasValue)
            {
                query += $"&endDateUtc={endDateUtc.Value:yyyy-MM-ddTHH:mm:ssZ}";
            }

            return await _httpClient.GetFromJsonAsync<WeatherHistoryResponseDto>(query);
        }
        catch
        {
            return null;
        }
    }

    public async Task<NationalSummaryDto?> GetNationalSummaryAsync()
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<NationalSummaryDto>("api/v1/weather/summary/national");
        }
        catch
        {
            return null;
        }
    }

    public string GetExportCsvUrl(int cityId, DateTime? startDateUtc, DateTime? endDateUtc)
    {
        var query = $"api/v1/weather/export?cityId={cityId}";
        if (startDateUtc.HasValue)
        {
            query += $"&startDateUtc={startDateUtc.Value:yyyy-MM-ddTHH:mm:ssZ}";
        }
        if (endDateUtc.HasValue)
        {
            query += $"&endDateUtc={endDateUtc.Value:yyyy-MM-ddTHH:mm:ssZ}";
        }
        return query;
    }

    public async Task<SyncResultDto?> TriggerSyncAsync()
    {
        try
        {
            var response = await _httpClient.PostAsync("api/v1/weather/sync", null);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<SyncResultDto>();
            }
            return null;
        }
        catch
        {
            return null;
        }
    }

    public async Task<ApiDiagnosticDto?> GetDiagnosticsAsync()
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<ApiDiagnosticDto>("api/v1/weather/diagnostics");
        }
        catch
        {
            return null;
        }
    }
}
