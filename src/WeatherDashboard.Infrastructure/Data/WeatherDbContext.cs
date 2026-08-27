namespace WeatherDashboard.Infrastructure.Data;

using Microsoft.EntityFrameworkCore;
using WeatherDashboard.Domain.Entities;
using WeatherDashboard.Infrastructure.Data.Configurations;

public class WeatherDbContext : DbContext
{
    public WeatherDbContext(DbContextOptions<WeatherDbContext> options) : base(options)
    {
    }

    public DbSet<City> Cities => Set<City>();
    public DbSet<WeatherReading> WeatherReadings => Set<WeatherReading>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfiguration(new CityConfiguration());
        modelBuilder.ApplyConfiguration(new WeatherReadingConfiguration());
    }
}
