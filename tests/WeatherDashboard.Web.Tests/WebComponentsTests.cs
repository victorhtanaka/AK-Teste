namespace WeatherDashboard.Web.Tests;

using Bunit;
using FluentAssertions;
using Microsoft.AspNetCore.Components;
using WeatherDashboard.Application.DTOs;
using WeatherDashboard.Web.Components;
using Xunit;

public class WebComponentsTests : TestContext
{
    [Fact]
    public void StatCard_Should_Render_Title_Value_And_Unit()
    {
        var cut = RenderComponent<StatCard>(parameters => parameters
            .Add(p => p.Title, "Temperatura Máxima")
            .Add(p => p.Value, "32.5")
            .Add(p => p.Unit, "°C")
            .Add(p => p.IconType, "temp-max")
            .Add(p => p.Subtitle, "Pico registrado hoje")
        );

        cut.Find(".stat-card-title").TextContent.Should().Be("Temperatura Máxima");
        cut.Find(".stat-card-value").TextContent.Should().Be("32.5");
        cut.Find(".stat-card-unit").TextContent.Should().Be("°C");
        cut.Find(".stat-card-footer").TextContent.Should().Contain("Pico registrado hoje");
        cut.Find("svg").Should().NotBeNull();
    }

    [Fact]
    public void CitySelector_Should_Render_Options_And_Fire_Callback_On_Select()
    {
        var cities = new List<CityDto>
        {
            new CityDto { Id = 7, Name = "Brasília", State = "DF" },
            new CityDto { Id = 25, Name = "São Paulo", State = "SP" }
        };

        CityDto? selectedCity = null;

        var cut = RenderComponent<CitySelector>(parameters => parameters
            .Add(p => p.Cities, cities)
            .Add(p => p.SelectedCityId, 7)
            .Add(p => p.OnCitySelected, EventCallback.Factory.Create<CityDto>(this, c => selectedCity = c))
        );

        var select = cut.Find("select");
        select.Should().NotBeNull();
        cut.FindAll("option").Should().HaveCount(2);

        // Dispara mudança para São Paulo
        select.Change("25");

        selectedCity.Should().NotBeNull();
        selectedCity!.Id.Should().Be(25);
        selectedCity.Name.Should().Be("São Paulo");
    }

    [Fact]
    public void CurrentWeatherCard_Should_Render_Current_Temperature_And_Description()
    {
        var city = new CityDto { Id = 7, Name = "Brasília", State = "DF" };
        var weather = new WeatherReadingDto
        {
            CityId = 7,
            CityName = "Brasília",
            State = "DF",
            TemperatureC = 27.5,
            FeelsLikeC = 28.0,
            Humidity = 50,
            WindSpeedMs = 4.0,
            PressureHpa = 1013.0,
            WeatherDescription = "Céu Limpo",
            WeatherIcon = "01d",
            CollectedAtUtc = DateTime.UtcNow
        };

        var cut = RenderComponent<CurrentWeatherCard>(parameters => parameters
            .Add(p => p.City, city)
            .Add(p => p.CurrentWeather, weather)
        );

        cut.Find(".hero-city-info h2").TextContent.Should().Contain("Brasília");
        cut.Find(".hero-condition-badge").TextContent.Should().Contain("Céu Limpo");
        cut.Find(".hero-temp-value").TextContent.Should().Be(27.5.ToString("F1"));
    }

    [Fact]
    public void TemperatureChart_Should_Render_Native_Svg_Paths_When_Data_Provided()
    {
        var points = new List<WeatherHistoryPointDto>
        {
            new WeatherHistoryPointDto { TimestampUtc = DateTime.UtcNow.AddHours(-2), TemperatureC = 24.0, FeelsLikeC = 25.0, WeatherDescription = "Sol" },
            new WeatherHistoryPointDto { TimestampUtc = DateTime.UtcNow.AddHours(-1), TemperatureC = 26.5, FeelsLikeC = 27.0, WeatherDescription = "Sol" },
            new WeatherHistoryPointDto { TimestampUtc = DateTime.UtcNow, TemperatureC = 28.0, FeelsLikeC = 29.0, WeatherDescription = "Sol" }
        };

        var cut = RenderComponent<TemperatureChart>(parameters => parameters
            .Add(p => p.HistoryPoints, points)
        );

        var svg = cut.Find("svg.native-svg-chart");
        svg.Should().NotBeNull();
        cut.FindAll("circle.chart-data-point").Should().HaveCount(3);
    }

    [Fact]
    public void HumidityChart_Should_Render_Native_Svg_And_Empty_State_When_Null()
    {
        var cutEmpty = RenderComponent<HumidityChart>(parameters => parameters
            .Add(p => p.HistoryPoints, Array.Empty<WeatherHistoryPointDto>())
        );

        cutEmpty.Find(".empty-chart-state").Should().NotBeNull();

        var points = new List<WeatherHistoryPointDto>
        {
            new WeatherHistoryPointDto { TimestampUtc = DateTime.UtcNow, Humidity = 65, WindSpeedMs = 3.5, PressureHpa = 1013 }
        };

        var cutWithData = RenderComponent<HumidityChart>(parameters => parameters
            .Add(p => p.HistoryPoints, points)
        );

        cutWithData.Find("svg.native-svg-chart").Should().NotBeNull();
    }
}
