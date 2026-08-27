namespace WeatherDashboard.Api.IntegrationTests;

using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using WeatherDashboard.Application.DTOs;
using Xunit;

public class ApiEndpointsIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public ApiEndpointsIntegrationTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetCities_Should_Return_200_With_27_Capitals()
    {
        var response = await _client.GetAsync("/api/v1/cities");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var cities = await response.Content.ReadFromJsonAsync<List<CityDto>>();

        cities.Should().NotBeNull();
        cities!.Count.Should().Be(27);
        cities.Should().Contain(c => c.Name == "Brasília" && c.State == "DF");
        cities.Should().Contain(c => c.Name == "São Paulo" && c.State == "SP");
        cities.Should().Contain(c => c.Name == "Curitiba" && c.State == "PR");
    }

    [Fact]
    public async Task GetCityById_Should_Return_200_When_Exists()
    {
        var response = await _client.GetAsync("/api/v1/cities/7");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var city = await response.Content.ReadFromJsonAsync<CityDto>();

        city.Should().NotBeNull();
        city!.Name.Should().Be("Brasília");
        city.State.Should().Be("DF");
    }

    [Fact]
    public async Task GetCityById_Should_Return_404_When_NotFound()
    {
        var response = await _client.GetAsync("/api/v1/cities/999");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetCurrentWeather_Should_Return_200_When_Data_Exists()
    {
        var response = await _client.GetAsync("/api/v1/weather/current?cityId=7");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var weather = await response.Content.ReadFromJsonAsync<WeatherReadingDto>();

        weather.Should().NotBeNull();
        weather!.CityName.Should().Be("Brasília");
        weather.TemperatureC.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task GetTodayStats_Should_Return_200_And_Computed_Stats()
    {
        var response = await _client.GetAsync("/api/v1/weather/stats/today?cityId=7");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var stats = await response.Content.ReadFromJsonAsync<TodayStatsDto>();

        stats.Should().NotBeNull();
        stats!.CityName.Should().Be("Brasília");
        stats.TotalReadings.Should().BeGreaterThanOrEqualTo(1);
    }

    [Fact]
    public async Task GetWeatherHistory_Should_Return_200_With_History_Points()
    {
        var response = await _client.GetAsync("/api/v1/weather/history?cityId=7");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var history = await response.Content.ReadFromJsonAsync<WeatherHistoryResponseDto>();

        history.Should().NotBeNull();
        history!.CityName.Should().Be("Brasília");
        history.Points.Should().NotBeEmpty();
    }

    [Fact]
    public async Task HealthCheck_Should_Return_Healthy()
    {
        var response = await _client.GetAsync("/health");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Be("Healthy");
    }
}
