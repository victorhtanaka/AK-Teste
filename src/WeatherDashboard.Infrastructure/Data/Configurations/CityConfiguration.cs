namespace WeatherDashboard.Infrastructure.Data.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WeatherDashboard.Domain.Entities;

public class CityConfiguration : IEntityTypeConfiguration<City>
{
    public void Configure(EntityTypeBuilder<City> builder)
    {
        builder.ToTable("Cities");

        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id).ValueGeneratedOnAdd();

        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(c => c.State)
            .IsRequired()
            .HasMaxLength(2);

        builder.Property(c => c.Latitude)
            .IsRequired();

        builder.Property(c => c.Longitude)
            .IsRequired();

        builder.Property(c => c.OpenWeatherCityId)
            .HasMaxLength(50);

        builder.HasIndex(c => new { c.Name, c.State }).IsUnique();

        // Seed das 27 Capitais Brasileiras
        builder.HasData(
            new City { Id = 1, Name = "Rio Branco", State = "AC", Latitude = -9.97499, Longitude = -67.8243 },
            new City { Id = 2, Name = "Maceió", State = "AL", Latitude = -9.66599, Longitude = -35.735 },
            new City { Id = 3, Name = "Macapá", State = "AP", Latitude = 0.0389, Longitude = -51.0664 },
            new City { Id = 4, Name = "Manaus", State = "AM", Latitude = -3.10194, Longitude = -60.025 },
            new City { Id = 5, Name = "Salvador", State = "BA", Latitude = -12.9711, Longitude = -38.5108 },
            new City { Id = 6, Name = "Fortaleza", State = "CE", Latitude = -3.71722, Longitude = -38.5431 },
            new City { Id = 7, Name = "Brasília", State = "DF", Latitude = -15.7797, Longitude = -47.9297 },
            new City { Id = 8, Name = "Vitória", State = "ES", Latitude = -20.3194, Longitude = -40.3378 },
            new City { Id = 9, Name = "Goiânia", State = "GO", Latitude = -16.6869, Longitude = -49.2648 },
            new City { Id = 10, Name = "São Luís", State = "MA", Latitude = -2.52972, Longitude = -44.3028 },
            new City { Id = 11, Name = "Cuiabá", State = "MT", Latitude = -15.5961, Longitude = -56.0967 },
            new City { Id = 12, Name = "Campo Grande", State = "MS", Latitude = -20.4428, Longitude = -54.6464 },
            new City { Id = 13, Name = "Belo Horizonte", State = "MG", Latitude = -19.9208, Longitude = -43.9378 },
            new City { Id = 14, Name = "Belém", State = "PA", Latitude = -1.45583, Longitude = -48.5039 },
            new City { Id = 15, Name = "João Pessoa", State = "PB", Latitude = -7.115, Longitude = -34.8631 },
            new City { Id = 16, Name = "Curitiba", State = "PR", Latitude = -25.4278, Longitude = -49.2731 },
            new City { Id = 17, Name = "Recife", State = "PE", Latitude = -8.05389, Longitude = -34.8811 },
            new City { Id = 18, Name = "Teresina", State = "PI", Latitude = -5.08917, Longitude = -42.8019 },
            new City { Id = 19, Name = "Rio de Janeiro", State = "RJ", Latitude = -22.9068, Longitude = -43.1729 },
            new City { Id = 20, Name = "Natal", State = "RN", Latitude = -5.795, Longitude = -35.2094 },
            new City { Id = 21, Name = "Porto Alegre", State = "RS", Latitude = -30.0331, Longitude = -51.23 },
            new City { Id = 22, Name = "Porto Velho", State = "RO", Latitude = -8.76194, Longitude = -63.9039 },
            new City { Id = 23, Name = "Boa Vista", State = "RR", Latitude = 2.81972, Longitude = -60.6733 },
            new City { Id = 24, Name = "Florianópolis", State = "SC", Latitude = -27.5954, Longitude = -48.548 },
            new City { Id = 25, Name = "São Paulo", State = "SP", Latitude = -23.5475, Longitude = -46.6361 },
            new City { Id = 26, Name = "Aracaju", State = "SE", Latitude = -10.9111, Longitude = -37.0717 },
            new City { Id = 27, Name = "Palmas", State = "TO", Latitude = -10.2128, Longitude = -48.3603 }
        );
    }
}
