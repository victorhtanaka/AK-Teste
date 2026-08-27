namespace WeatherDashboard.Api.IntegrationTests;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using WeatherDashboard.Domain.Entities;
using WeatherDashboard.Domain.Interfaces;
using WeatherDashboard.Infrastructure.Data;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string _dbName = Guid.NewGuid().ToString();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove o DbContext existente
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<WeatherDbContext>));
            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            // Adiciona DbContext com InMemory único para os testes de integração
            services.AddDbContext<WeatherDbContext>(options =>
            {
                options.UseInMemoryDatabase(_dbName);
            });

            // Constrói o provider para garantir seed dos dados de teste
            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<WeatherDbContext>();
            db.Database.EnsureCreated();

            // Adiciona leituras de teste se não existirem
            if (!db.WeatherReadings.Any())
            {
                var now = DateTime.UtcNow;
                db.WeatherReadings.AddRange(
                    new WeatherReading
                    {
                        CityId = 7, // Brasília
                        CollectedAtUtc = now.AddHours(-2),
                        TemperatureC = 26.5,
                        FeelsLikeC = 26.0,
                        TempMinC = 22.0,
                        TempMaxC = 28.0,
                        Humidity = 55,
                        PressureHpa = 1014.0,
                        WindSpeedMs = 3.5,
                        WeatherDescription = "Céu limpo",
                        WeatherIcon = "01d"
                    },
                    new WeatherReading
                    {
                        CityId = 7,
                        CollectedAtUtc = now,
                        TemperatureC = 27.2,
                        FeelsLikeC = 26.8,
                        TempMinC = 22.0,
                        TempMaxC = 28.5,
                        Humidity = 50,
                        PressureHpa = 1013.0,
                        WindSpeedMs = 4.0,
                        WeatherDescription = "Ensolarado",
                        WeatherIcon = "01d"
                    }
                );
                db.SaveChanges();
            }
        });
    }
}
