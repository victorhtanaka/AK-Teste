namespace WeatherDashboard.Application.Services;

using System.Globalization;
using System.Text;
using FluentValidation;
using WeatherDashboard.Application.DTOs;
using WeatherDashboard.Application.Interfaces;
using WeatherDashboard.Domain.Interfaces;

public class WeatherService : IWeatherService
{
    private readonly IWeatherReadingRepository _weatherReadingRepository;
    private readonly ICityRepository _cityRepository;
    private readonly IValidator<WeatherHistoryFilterDto> _filterValidator;

    public WeatherService(
        IWeatherReadingRepository weatherReadingRepository,
        ICityRepository cityRepository,
        IValidator<WeatherHistoryFilterDto> filterValidator)
    {
        _weatherReadingRepository = weatherReadingRepository;
        _cityRepository = cityRepository;
        _filterValidator = filterValidator;
    }

    public async Task<WeatherReadingDto?> GetCurrentWeatherAsync(int cityId, CancellationToken cancellationToken = default)
    {
        var city = await _cityRepository.GetByIdAsync(cityId, cancellationToken);
        if (city == null) return null;

        var reading = await _weatherReadingRepository.GetLatestByCityIdAsync(cityId, cancellationToken);
        if (reading == null) return null;

        return new WeatherReadingDto
        {
            Id = reading.Id,
            CityId = city.Id,
            CityName = city.Name,
            State = city.State,
            CollectedAtUtc = reading.CollectedAtUtc,
            TemperatureC = reading.TemperatureC,
            FeelsLikeC = reading.FeelsLikeC,
            TempMinC = reading.TempMinC,
            TempMaxC = reading.TempMaxC,
            Humidity = reading.Humidity,
            PressureHpa = reading.PressureHpa,
            WindSpeedMs = reading.WindSpeedMs,
            WeatherDescription = reading.WeatherDescription,
            WeatherIcon = reading.WeatherIcon
        };
    }

    public async Task<WeatherHistoryResponseDto> GetWeatherHistoryAsync(WeatherHistoryFilterDto filter, CancellationToken cancellationToken = default)
    {
        var validationResult = await _filterValidator.ValidateAsync(filter, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        var city = await _cityRepository.GetByIdAsync(filter.CityId, cancellationToken)
            ?? throw new KeyNotFoundException($"Cidade com ID {filter.CityId} não encontrada.");

        var now = DateTime.UtcNow;
        var startUtc = filter.StartDateUtc ?? now.Date.AddDays(-6);
        var endUtc = filter.EndDateUtc ?? now;

        if (startUtc > endUtc)
        {
            startUtc = endUtc.AddDays(-1);
        }

        var readings = await _weatherReadingRepository.GetByCityAndDateRangeAsync(filter.CityId, startUtc, endUtc, cancellationToken);

        var points = readings.Select(r => new WeatherHistoryPointDto
        {
            TimestampUtc = r.CollectedAtUtc,
            TemperatureC = Math.Round(r.TemperatureC, 1),
            FeelsLikeC = Math.Round(r.FeelsLikeC, 1),
            TempMinC = Math.Round(r.TempMinC, 1),
            TempMaxC = Math.Round(r.TempMaxC, 1),
            Humidity = r.Humidity,
            WindSpeedMs = Math.Round(r.WindSpeedMs, 1),
            PressureHpa = Math.Round(r.PressureHpa, 1),
            WeatherDescription = r.WeatherDescription,
            WeatherIcon = r.WeatherIcon
        }).ToList();

        return new WeatherHistoryResponseDto
        {
            CityId = city.Id,
            CityName = city.Name,
            State = city.State,
            StartUtc = startUtc,
            EndUtc = endUtc,
            Points = points
        };
    }

    public async Task<TodayStatsDto> GetTodayStatsAsync(int cityId, CancellationToken cancellationToken = default)
    {
        var city = await _cityRepository.GetByIdAsync(cityId, cancellationToken)
            ?? throw new KeyNotFoundException($"Cidade com ID {cityId} não encontrada.");

        var todayUtcDate = DateTime.UtcNow.Date;
        var readings = await _weatherReadingRepository.GetTodayReadingsByCityIdAsync(cityId, todayUtcDate, cancellationToken);

        if (readings.Count == 0)
        {
            var latest = await _weatherReadingRepository.GetLatestByCityIdAsync(cityId, cancellationToken);
            if (latest != null)
            {
                return new TodayStatsDto
                {
                    CityId = city.Id,
                    CityName = city.Name,
                    State = city.State,
                    DateUtc = latest.CollectedAtUtc.Date,
                    TempMaxC = Math.Round(latest.TempMaxC, 1),
                    TempMinC = Math.Round(latest.TempMinC, 1),
                    TempAvgC = Math.Round(latest.TemperatureC, 1),
                    FeelsLikeAvgC = Math.Round(latest.FeelsLikeC, 1),
                    HumidityAvg = Math.Round((double)latest.Humidity, 1),
                    PressureAvgHpa = Math.Round(latest.PressureHpa, 1),
                    WindSpeedAvgMs = Math.Round(latest.WindSpeedMs, 1),
                    DominantWeatherDescription = latest.WeatherDescription,
                    DominantWeatherIcon = latest.WeatherIcon,
                    TotalReadings = 1,
                    LastUpdatedUtc = latest.CollectedAtUtc
                };
            }

            return new TodayStatsDto
            {
                CityId = city.Id,
                CityName = city.Name,
                State = city.State,
                DateUtc = todayUtcDate,
                TotalReadings = 0
            };
        }

        var tempMax = readings.Max(r => r.TempMaxC);
        var tempMin = readings.Min(r => r.TempMinC);
        var tempAvg = readings.Average(r => r.TemperatureC);
        var feelsLikeAvg = readings.Average(r => r.FeelsLikeC);
        var humidityAvg = readings.Average(r => r.Humidity);
        var pressureAvg = readings.Average(r => r.PressureHpa);
        var windSpeedAvg = readings.Average(r => r.WindSpeedMs);
        var latestReading = readings.OrderByDescending(r => r.CollectedAtUtc).First();

        var dominantDescription = readings
            .GroupBy(r => r.WeatherDescription)
            .OrderByDescending(g => g.Count())
            .First().Key;

        var dominantIcon = readings
            .FirstOrDefault(r => r.WeatherDescription == dominantDescription)?.WeatherIcon
            ?? latestReading.WeatherIcon;

        return new TodayStatsDto
        {
            CityId = city.Id,
            CityName = city.Name,
            State = city.State,
            DateUtc = todayUtcDate,
            TempMaxC = Math.Round(tempMax, 1),
            TempMinC = Math.Round(tempMin, 1),
            TempAvgC = Math.Round(tempAvg, 1),
            FeelsLikeAvgC = Math.Round(feelsLikeAvg, 1),
            HumidityAvg = Math.Round(humidityAvg, 1),
            PressureAvgHpa = Math.Round(pressureAvg, 1),
            WindSpeedAvgMs = Math.Round(windSpeedAvg, 1),
            DominantWeatherDescription = dominantDescription,
            DominantWeatherIcon = dominantIcon,
            TotalReadings = readings.Count,
            LastUpdatedUtc = latestReading.CollectedAtUtc
        };
    }

    public async Task<NationalSummaryDto> GetNationalSummaryAsync(CancellationToken cancellationToken = default)
    {
        var cities = await _cityRepository.GetAllAsync(cancellationToken);
        var latestReadings = new List<(Domain.Entities.City City, Domain.Entities.WeatherReading Reading)>();

        foreach (var city in cities)
        {
            var reading = await _weatherReadingRepository.GetLatestByCityIdAsync(city.Id, cancellationToken);
            if (reading != null)
            {
                latestReadings.Add((city, reading));
            }
        }

        if (latestReadings.Count == 0)
        {
            return new NationalSummaryDto
            {
                TotalMonitoredCapitals = cities.Count,
                TotalReadingsToday = 0,
                CalculatedAtUtc = DateTime.UtcNow
            };
        }

        var warmest = latestReadings.OrderByDescending(r => r.Reading.TemperatureC).First();
        var coldest = latestReadings.OrderBy(r => r.Reading.TemperatureC).First();
        var avgTemp = latestReadings.Average(r => r.Reading.TemperatureC);
        var avgHumidity = latestReadings.Average(r => r.Reading.Humidity);

        return new NationalSummaryDto
        {
            TotalMonitoredCapitals = cities.Count,
            TotalReadingsToday = latestReadings.Count,
            NationalAvgTempC = Math.Round(avgTemp, 1),
            NationalAvgHumidity = Math.Round(avgHumidity, 0),
            CalculatedAtUtc = DateTime.UtcNow,
            WarmestCity = new CityTempSummaryDto
            {
                CityId = warmest.City.Id,
                CityName = warmest.City.Name,
                State = warmest.City.State,
                TemperatureC = Math.Round(warmest.Reading.TemperatureC, 1),
                WeatherDescription = warmest.Reading.WeatherDescription,
                WeatherIcon = warmest.Reading.WeatherIcon
            },
            ColdestCity = new CityTempSummaryDto
            {
                CityId = coldest.City.Id,
                CityName = coldest.City.Name,
                State = coldest.City.State,
                TemperatureC = Math.Round(coldest.Reading.TemperatureC, 1),
                WeatherDescription = coldest.Reading.WeatherDescription,
                WeatherIcon = coldest.Reading.WeatherIcon
            }
        };
    }

    public async Task<byte[]> ExportHistoryCsvAsync(WeatherHistoryFilterDto filter, CancellationToken cancellationToken = default)
    {
        var history = await GetWeatherHistoryAsync(filter, cancellationToken);
        var sb = new StringBuilder();
        sb.AppendLine("DataHora_UTC;DataHora_Local;Capital;UF;Temperatura_C;Sensacao_C;TempMin_C;TempMax_C;Umidade_Pct;Vento_Ms;Pressao_Hpa;Condicao");

        foreach (var p in history.Points)
        {
            sb.AppendLine(string.Format(CultureInfo.InvariantCulture,
                "{0:yyyy-MM-dd HH:mm:ss};{1:yyyy-MM-dd HH:mm:ss};{2};{3};{4:F1};{5:F1};{6:F1};{7:F1};{8};{9:F1};{10:F0};{11}",
                p.TimestampUtc,
                p.TimestampUtc.ToLocalTime(),
                history.CityName,
                history.State,
                p.TemperatureC,
                p.FeelsLikeC,
                p.TempMinC,
                p.TempMaxC,
                p.Humidity,
                p.WindSpeedMs,
                p.PressureHpa,
                p.WeatherDescription));
        }

        return Encoding.UTF8.GetPreamble().Concat(Encoding.UTF8.GetBytes(sb.ToString())).ToArray();
    }
}
