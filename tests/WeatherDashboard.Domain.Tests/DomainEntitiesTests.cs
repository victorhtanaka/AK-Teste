namespace WeatherDashboard.Domain.Tests;

using FluentAssertions;
using WeatherDashboard.Domain.Entities;
using Xunit;

public class DomainEntitiesTests
{
    [Fact]
    public void City_Should_Instantiate_With_Correct_Properties()
    {
        var city = new City
        {
            Id = 16,
            Name = "Curitiba",
            State = "PR",
            Latitude = -25.4278,
            Longitude = -49.2731,
            OpenWeatherCityId = "3464975"
        };

        city.Id.Should().Be(16);
        city.Name.Should().Be("Curitiba");
        city.State.Should().Be("PR");
        city.Latitude.Should().Be(-25.4278);
        city.Longitude.Should().Be(-49.2731);
        city.Readings.Should().NotBeNull().And.BeEmpty();
    }

    [Fact]
    public void WeatherReading_Should_Associate_With_City()
    {
        var city = new City { Id = 25, Name = "São Paulo", State = "SP" };
        var reading = new WeatherReading
        {
            Id = 100,
            CityId = city.Id,
            City = city,
            CollectedAtUtc = DateTime.UtcNow,
            TemperatureC = 23.5,
            FeelsLikeC = 24.1,
            TempMinC = 20.0,
            TempMaxC = 26.0,
            Humidity = 72,
            PressureHpa = 1015.0,
            WindSpeedMs = 4.2,
            WeatherDescription = "Parcialmente nublado",
            WeatherIcon = "03d"
        };

        reading.CityId.Should().Be(25);
        reading.City.Name.Should().Be("São Paulo");
        reading.TemperatureC.Should().Be(23.5);
        reading.Humidity.Should().Be(72);
    }
}
