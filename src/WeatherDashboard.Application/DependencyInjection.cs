namespace WeatherDashboard.Application;

using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using WeatherDashboard.Application.Interfaces;
using WeatherDashboard.Application.Services;
using WeatherDashboard.Application.Validators;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining<WeatherHistoryFilterValidator>();
        services.AddScoped<ICityService, CityService>();
        services.AddScoped<IWeatherService, WeatherService>();
        services.AddScoped<IWeatherSyncService, WeatherSyncService>();

        return services;
    }
}
