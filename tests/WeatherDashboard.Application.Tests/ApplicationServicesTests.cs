namespace WeatherDashboard.Application.Tests;

using FluentAssertions;
using FluentValidation;
using Moq;
using WeatherDashboard.Application.DTOs;
using WeatherDashboard.Application.Services;
using WeatherDashboard.Application.Validators;
using WeatherDashboard.Domain.Entities;
using WeatherDashboard.Domain.Interfaces;
using Xunit;

public class ApplicationServicesTests
{
    private readonly Mock<ICityRepository> _cityRepoMock = new();
    private readonly Mock<IWeatherReadingRepository> _readingRepoMock = new();
    private readonly Mock<IOpenWeatherClient> _owmClientMock = new();
    private readonly IValidator<WeatherHistoryFilterDto> _validator = new WeatherHistoryFilterValidator();

    [Fact]
    public async Task CityService_GetAllCities_Should_Return_Mapped_DTOs()
    {
        var cities = new List<City>
        {
            new City { Id = 1, Name = "Rio Branco", State = "AC", Latitude = -9.9, Longitude = -67.8 },
            new City { Id = 2, Name = "Maceió", State = "AL", Latitude = -9.6, Longitude = -35.7 }
        };

        _cityRepoMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(cities);

        var service = new CityService(_cityRepoMock.Object);
        var result = await service.GetAllCitiesAsync();

        result.Should().HaveCount(2);
        result[0].Name.Should().Be("Rio Branco");
        result[0].FullDisplayName.Should().Be("Rio Branco - AC");
    }

    [Fact]
    public async Task WeatherService_GetCurrentWeather_Should_Return_Latest_Reading()
    {
        var city = new City { Id = 16, Name = "Curitiba", State = "PR" };
        var reading = new WeatherReading
        {
            Id = 1,
            CityId = 16,
            CollectedAtUtc = DateTime.UtcNow,
            TemperatureC = 18.5,
            FeelsLikeC = 17.8,
            Humidity = 80,
            WeatherDescription = "Nublado",
            WeatherIcon = "04d"
        };

        _cityRepoMock.Setup(r => r.GetByIdAsync(16, It.IsAny<CancellationToken>()))
            .ReturnsAsync(city);
        _readingRepoMock.Setup(r => r.GetLatestByCityIdAsync(16, It.IsAny<CancellationToken>()))
            .ReturnsAsync(reading);

        var service = new WeatherService(_readingRepoMock.Object, _cityRepoMock.Object, _validator);
        var result = await service.GetCurrentWeatherAsync(16);

        result.Should().NotBeNull();
        result!.CityName.Should().Be("Curitiba");
        result.TemperatureC.Should().Be(18.5);
        result.WeatherDescription.Should().Be("Nublado");
    }

    [Fact]
    public async Task WeatherService_GetTodayStats_Should_Calculate_Aggregations_Correctly()
    {
        var city = new City { Id = 25, Name = "São Paulo", State = "SP" };
        var today = DateTime.UtcNow.Date;
        var readings = new List<WeatherReading>
        {
            new WeatherReading { CityId = 25, CollectedAtUtc = today.AddHours(8), TemperatureC = 20.0, FeelsLikeC = 20.0, TempMinC = 18.0, TempMaxC = 22.0, Humidity = 70, PressureHpa = 1014, WindSpeedMs = 3.0, WeatherDescription = "Sol", WeatherIcon = "01d" },
            new WeatherReading { CityId = 25, CollectedAtUtc = today.AddHours(14), TemperatureC = 30.0, FeelsLikeC = 32.0, TempMinC = 20.0, TempMaxC = 31.0, Humidity = 50, PressureHpa = 1012, WindSpeedMs = 5.0, WeatherDescription = "Sol", WeatherIcon = "01d" },
            new WeatherReading { CityId = 25, CollectedAtUtc = today.AddHours(20), TemperatureC = 25.0, FeelsLikeC = 26.0, TempMinC = 19.0, TempMaxC = 26.0, Humidity = 60, PressureHpa = 1013, WindSpeedMs = 4.0, WeatherDescription = "Nublado", WeatherIcon = "03d" }
        };

        _cityRepoMock.Setup(r => r.GetByIdAsync(25, It.IsAny<CancellationToken>()))
            .ReturnsAsync(city);
        _readingRepoMock.Setup(r => r.GetTodayReadingsByCityIdAsync(25, It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(readings);

        var service = new WeatherService(_readingRepoMock.Object, _cityRepoMock.Object, _validator);
        var result = await service.GetTodayStatsAsync(25);

        result.Should().NotBeNull();
        result.CityName.Should().Be("São Paulo");
        result.TempMaxC.Should().Be(31.0);
        result.TempMinC.Should().Be(18.0);
        result.TempAvgC.Should().Be(25.0); // (20 + 30 + 25) / 3
        result.HumidityAvg.Should().Be(60.0); // (70 + 50 + 60) / 3
        result.DominantWeatherDescription.Should().Be("Sol"); // 2 'Sol' vs 1 'Nublado'
        result.TotalReadings.Should().Be(3);
    }

    [Fact]
    public async Task WeatherSyncService_Should_Iterate_All_Cities_And_Tolerate_Individual_Failure()
    {
        var cities = new List<City>
        {
            new City { Id = 1, Name = "Cidade A", Latitude = 10, Longitude = 10 },
            new City { Id = 2, Name = "Cidade B (Falha)", Latitude = 20, Longitude = 20 },
            new City { Id = 3, Name = "Cidade C", Latitude = 30, Longitude = 30 }
        };

        _cityRepoMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(cities);

        // Cidade 1 ok
        _owmClientMock.Setup(c => c.GetCurrentWeatherByCoordinatesAsync(10, 10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new OpenWeatherResponseModel { TemperatureC = 25, WeatherDescription = "Limpo", WeatherIcon = "01d", TimestampUtc = DateTime.UtcNow });

        // Cidade 2 lança exceção (ex: timeout ou erro transitório)
        _owmClientMock.Setup(c => c.GetCurrentWeatherByCoordinatesAsync(20, 20, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new HttpRequestException("Timeout"));

        // Cidade 3 ok
        _owmClientMock.Setup(c => c.GetCurrentWeatherByCoordinatesAsync(30, 30, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new OpenWeatherResponseModel { TemperatureC = 28, WeatherDescription = "Nublado", WeatherIcon = "03d", TimestampUtc = DateTime.UtcNow });

        var loggerMock = new Mock<Microsoft.Extensions.Logging.ILogger<WeatherSyncService>>();
        var syncService = new WeatherSyncService(_cityRepoMock.Object, _readingRepoMock.Object, _owmClientMock.Object, loggerMock.Object);

        var result = await syncService.SyncAllCitiesWeatherAsync();

        result.TotalCities.Should().Be(3);
        result.SuccessCount.Should().Be(2);
        result.FailureCount.Should().Be(1);
        result.Errors.Should().HaveCount(1);

        _readingRepoMock.Verify(r => r.AddRangeAsync(It.Is<IEnumerable<WeatherReading>>(list => list.Count() == 2), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public void Validator_Should_Fail_When_StartDate_Is_After_EndDate()
    {
        var filter = new WeatherHistoryFilterDto
        {
            CityId = 1,
            StartDateUtc = DateTime.UtcNow.AddDays(2),
            EndDateUtc = DateTime.UtcNow
        };

        var result = _validator.Validate(filter);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage.Contains("A data inicial não pode ser posterior"));
    }
}
