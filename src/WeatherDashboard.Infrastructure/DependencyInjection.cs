namespace WeatherDashboard.Infrastructure;

using System;
using System.Net.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;
using WeatherDashboard.Domain.Interfaces;
using WeatherDashboard.Infrastructure.Clients;
using WeatherDashboard.Infrastructure.Data;
using WeatherDashboard.Infrastructure.Data.Repositories;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<WeatherDbContext>(options =>
        {
            if (string.IsNullOrWhiteSpace(connectionString) || connectionString.Contains("InMemory", StringComparison.OrdinalIgnoreCase))
            {
                options.UseInMemoryDatabase("WeatherDashboardDb");
            }
            else
            {
                options.UseSqlServer(connectionString, sqlOptions =>
                {
                    sqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 5,
                        maxRetryDelay: TimeSpan.FromSeconds(10),
                        errorNumbersToAdd: null);
                });
            }
        });

        services.AddScoped<ICityRepository, CityRepository>();
        services.AddScoped<IWeatherReadingRepository, WeatherReadingRepository>();

        // Configuração de HttpClient com Polly (Retry com backoff exponencial)
        services.AddHttpClient<IOpenWeatherClient, OpenWeatherMapClient>(client =>
        {
            var baseUrl = configuration["OpenWeather:BaseUrl"] ?? "https://api.openweathermap.org/data/2.5/";
            if (!baseUrl.EndsWith('/')) baseUrl += "/";
            client.BaseAddress = new Uri(baseUrl);
            client.Timeout = TimeSpan.FromSeconds(15);
        })
        .AddPolicyHandler(GetRetryPolicy());

        return services;
    }

    private static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
            .WaitAndRetryAsync(
                retryCount: 3,
                sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))
            );
    }
}
