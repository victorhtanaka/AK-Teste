namespace WeatherDashboard.Infrastructure.Data.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WeatherDashboard.Domain.Entities;

public class WeatherReadingConfiguration : IEntityTypeConfiguration<WeatherReading>
{
    public void Configure(EntityTypeBuilder<WeatherReading> builder)
    {
        builder.ToTable("WeatherReadings");

        builder.HasKey(r => r.Id);
        builder.Property(r => r.Id).ValueGeneratedOnAdd();

        builder.Property(r => r.CollectedAtUtc)
            .IsRequired();

        builder.Property(r => r.TemperatureC)
            .IsRequired();

        builder.Property(r => r.FeelsLikeC)
            .IsRequired();

        builder.Property(r => r.TempMinC)
            .IsRequired();

        builder.Property(r => r.TempMaxC)
            .IsRequired();

        builder.Property(r => r.Humidity)
            .IsRequired();

        builder.Property(r => r.PressureHpa)
            .IsRequired();

        builder.Property(r => r.WindSpeedMs)
            .IsRequired();

        builder.Property(r => r.WeatherDescription)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(r => r.WeatherIcon)
            .IsRequired()
            .HasMaxLength(20);

        builder.HasOne(r => r.City)
            .WithMany(c => c.Readings)
            .HasForeignKey(r => r.CityId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(r => new { r.CityId, r.CollectedAtUtc });
    }
}
