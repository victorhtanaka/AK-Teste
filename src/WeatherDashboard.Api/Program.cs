using Serilog;
using WeatherDashboard.Api.BackgroundServices;
using WeatherDashboard.Api.Extensions;
using WeatherDashboard.Application;
using WeatherDashboard.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Configuração estruturada do Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
    .WriteTo.File("logs/weather-dashboard-.log", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// Injeção de dependência das camadas Clean Architecture
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// Background Worker de coleta de 15 minutos
builder.Services.AddHostedService<WeatherCollectorWorker>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Weather Dashboard API - Capitais Brasileiras",
        Version = "v1",
        Description = "API RESTful para consulta e monitoramento do clima das 27 capitais brasileiras."
    });
});

builder.Services.AddHealthChecks();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Inicialização e Seed do Banco de Dados
await app.Services.InitializeDatabaseAsync();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Weather Dashboard API v1");
    });
    app.UseWebAssemblyDebugging();
}

app.UseSerilogRequestLogging();

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseRouting();
app.UseCors("AllowAll");

app.MapHealthChecks("/health");
app.MapControllers();
app.MapFallbackToFile("index.html");

try
{
    Log.Information("Iniciando Weather Dashboard API...");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Falha fatal na inicialização da aplicação.");
}
finally
{
    Log.CloseAndFlush();
}

public partial class Program { }
