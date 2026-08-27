namespace WeatherDashboard.Infrastructure.Clients;

using System.Diagnostics;
using System.Globalization;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using WeatherDashboard.Domain.Interfaces;

public class OpenWeatherMapClient : IOpenWeatherClient
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<OpenWeatherMapClient> _logger;

    public OpenWeatherMapClient(
        HttpClient httpClient,
        IConfiguration configuration,
        ILogger<OpenWeatherMapClient> logger)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _logger = logger;
    }

    public bool IsApiKeyConfigured
    {
        get
        {
            var apiKey = _configuration["OpenWeather:ApiKey"];
            return !string.IsNullOrWhiteSpace(apiKey) && !apiKey.Equals("YOUR_API_KEY", StringComparison.OrdinalIgnoreCase);
        }
    }

    public async Task<OpenWeatherResponseModel?> GetCurrentWeatherByCoordinatesAsync(
        double latitude,
        double longitude,
        CancellationToken cancellationToken = default)
    {
        if (!IsApiKeyConfigured)
        {
            return GenerateSimulatedWeather(latitude);
        }

        var apiKey = _configuration["OpenWeather:ApiKey"];
        var latStr = latitude.ToString(CultureInfo.InvariantCulture);
        var lonStr = longitude.ToString(CultureInfo.InvariantCulture);
        var url = $"weather?lat={latStr}&lon={lonStr}&appid={apiKey}&units=metric&lang=pt_br";

        try
        {
            var response = await _httpClient.GetAsync(url, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var payload = await response.Content.ReadFromJsonAsync<OwmApiResponse>(cancellationToken: cancellationToken);
                if (payload != null && payload.Main != null)
                {
                    var weatherInfo = payload.Weather?.FirstOrDefault();
                    return new OpenWeatherResponseModel
                    {
                        TemperatureC = payload.Main.Temp,
                        FeelsLikeC = payload.Main.FeelsLike,
                        TempMinC = payload.Main.TempMin,
                        TempMaxC = payload.Main.TempMax,
                        Humidity = payload.Main.Humidity,
                        PressureHpa = payload.Main.Pressure,
                        WindSpeedMs = payload.Wind?.Speed ?? 0,
                        WeatherDescription = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(weatherInfo?.Description ?? "Céu limpo"),
                        WeatherIcon = weatherInfo?.Icon ?? "01d",
                        TimestampUtc = DateTimeOffset.FromUnixTimeSeconds(payload.Dt > 0 ? payload.Dt : DateTimeOffset.UtcNow.ToUnixTimeSeconds()).UtcDateTime
                    };
                }
            }
            else
            {
                var body = await response.Content.ReadAsStringAsync(cancellationToken);
                _logger.LogWarning("OpenWeatherMap retornou status {Status}: {Body}", (int)response.StatusCode, body);
                return null;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro de comunicação com a OpenWeatherMap.");
            return null;
        }

        return null;
    }

    public async Task<(bool Success, int StatusCode, string Message)> TestConnectionAsync(CancellationToken cancellationToken = default)
    {
        if (!IsApiKeyConfigured)
        {
            return (false, 0, "Chave de API não informada nos segredos. O sistema está operando com o gerador simulado integrado.");
        }

        var apiKey = _configuration["OpenWeather:ApiKey"];
        var url = $"weather?lat=-15.7797&lon=-47.9297&appid={apiKey}&units=metric";
        try
        {
            var stopwatch = Stopwatch.StartNew();
            var response = await _httpClient.GetAsync(url, cancellationToken);
            stopwatch.Stop();

            var body = await response.Content.ReadAsStringAsync(cancellationToken);
            if (response.IsSuccessStatusCode)
            {
                return (true, (int)response.StatusCode, $"Conexão bem-sucedida com a OpenWeatherMap ({stopwatch.ElapsedMilliseconds}ms).");
            }

            return (false, (int)response.StatusCode, $"OpenWeatherMap respondeu HTTP {(int)response.StatusCode}: {body}");
        }
        catch (Exception ex)
        {
            return (false, 500, $"Falha de conexão com os servidores da OpenWeather: {ex.Message}");
        }
    }

    private static OpenWeatherResponseModel GenerateSimulatedWeather(double latitude)
    {
        var now = DateTime.UtcNow;
        var latFactor = Math.Abs(latitude);
        var baseTemp = 31.0 - (latFactor * 0.45);
        var hour = now.Hour;
        var dailyCycle = Math.Sin((hour - 6) * Math.PI / 12.0) * 3.5;
        var temp = Math.Round(baseTemp + dailyCycle, 1);
        var tempMin = Math.Round(temp - 2.0, 1);
        var tempMax = Math.Round(temp + 2.5, 1);
        var feels = Math.Round(temp + (latitude < -15 ? 0.4 : 1.5), 1);
        var humidity = Math.Clamp((int)(60 + (Math.Sin(hour) * 18)), 35, 95);
        var pressure = Math.Round(1013.0 + (Math.Cos(hour) * 3.0), 1);
        var wind = Math.Round(2.5 + (Math.Abs(Math.Sin(hour)) * 3.5), 1);

        string desc = humidity > 78 ? "Chuva passageira" : (humidity > 60 ? "Parcialmente nublado" : "Céu limpo");
        string icon = humidity > 78 ? "10d" : (humidity > 60 ? "03d" : "01d");

        return new OpenWeatherResponseModel
        {
            TemperatureC = temp,
            FeelsLikeC = feels,
            TempMinC = tempMin,
            TempMaxC = tempMax,
            Humidity = humidity,
            PressureHpa = pressure,
            WindSpeedMs = wind,
            WeatherDescription = desc,
            WeatherIcon = icon,
            TimestampUtc = now
        };
    }

    private class OwmApiResponse
    {
        [JsonPropertyName("main")]
        public OwmMain? Main { get; set; }

        [JsonPropertyName("weather")]
        public List<OwmWeather>? Weather { get; set; }

        [JsonPropertyName("wind")]
        public OwmWind? Wind { get; set; }

        [JsonPropertyName("dt")]
        public long Dt { get; set; }
    }

    private class OwmMain
    {
        [JsonPropertyName("temp")]
        public double Temp { get; set; }

        [JsonPropertyName("feels_like")]
        public double FeelsLike { get; set; }

        [JsonPropertyName("temp_min")]
        public double TempMin { get; set; }

        [JsonPropertyName("temp_max")]
        public double TempMax { get; set; }

        [JsonPropertyName("pressure")]
        public double Pressure { get; set; }

        [JsonPropertyName("humidity")]
        public int Humidity { get; set; }
    }

    private class OwmWeather
    {
        [JsonPropertyName("description")]
        public string Description { get; set; } = string.Empty;

        [JsonPropertyName("icon")]
        public string Icon { get; set; } = string.Empty;
    }

    private class OwmWind
    {
        [JsonPropertyName("speed")]
        public double Speed { get; set; }
    }
}
