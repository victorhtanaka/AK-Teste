namespace WeatherDashboard.Api.Extensions;

using Microsoft.EntityFrameworkCore;
using WeatherDashboard.Domain.Entities;
using WeatherDashboard.Infrastructure.Data;

public static class DatabaseInitializationExtensions
{
    public static async Task InitializeDatabaseAsync(this IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<WeatherDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("DatabaseInit");

        try
        {
            if (context.Database.IsSqlServer())
            {
                logger.LogInformation("Aplicando migrações pendentes no SQL Server...");
                await context.Database.MigrateAsync();
            }
            else
            {
                logger.LogInformation("Garantindo criação do banco em memória...");
                await context.Database.EnsureCreatedAsync();
            }

            // Se o banco não tiver leituras históricas (novo banco), popula dados históricos das últimas 48 horas
            if (!await context.WeatherReadings.AnyAsync())
            {
                logger.LogInformation("Populando dados climáticos históricos iniciais para as capitais...");
                await SeedHistoricalReadingsAsync(context);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro durante a inicialização do banco de dados.");
        }
    }

    private static async Task SeedHistoricalReadingsAsync(WeatherDbContext context)
    {
        var cities = await context.Cities.ToListAsync();
        if (cities.Count == 0) return;

        var readings = new List<WeatherReading>();
        var now = DateTime.UtcNow;

        // Cria pontos a cada hora pelas últimas 48 horas para cada capital
        foreach (var city in cities)
        {
            var latFactor = Math.Abs(city.Latitude);
            var baseTemp = 31.0 - (latFactor * 0.45);

            for (int i = 48; i >= 0; i--)
            {
                var timestamp = now.AddHours(-i);
                var hour = timestamp.Hour;
                var dailyCycle = Math.Sin((hour - 6) * Math.PI / 12.0) * 3.5;
                var temp = Math.Round(baseTemp + dailyCycle + ((city.Id % 5) * 0.3), 1);
                var tempMin = Math.Round(temp - 2.0, 1);
                var tempMax = Math.Round(temp + 2.5, 1);
                var feels = Math.Round(temp + (city.Latitude < -15 ? 0.4 : 1.5), 1);
                var humidity = Math.Clamp((int)(60 + (Math.Sin(hour + city.Id) * 18)), 35, 95);
                var pressure = Math.Round(1013.0 + (Math.Cos(hour) * 3.0), 1);
                var wind = Math.Round(2.5 + (Math.Abs(Math.Sin(hour + city.Id)) * 3.5), 1);

                string desc = humidity > 78 ? "Chuva passageira" : (humidity > 60 ? "Parcialmente nublado" : "Céu limpo");
                string icon = humidity > 78 ? "10d" : (humidity > 60 ? "03d" : "01d");

                readings.Add(new WeatherReading
                {
                    CityId = city.Id,
                    CollectedAtUtc = timestamp,
                    TemperatureC = temp,
                    FeelsLikeC = feels,
                    TempMinC = tempMin,
                    TempMaxC = tempMax,
                    Humidity = humidity,
                    PressureHpa = pressure,
                    WindSpeedMs = wind,
                    WeatherDescription = desc,
                    WeatherIcon = icon
                });
            }
        }

        await context.WeatherReadings.AddRangeAsync(readings);
        await context.SaveChangesAsync();
    }
}
