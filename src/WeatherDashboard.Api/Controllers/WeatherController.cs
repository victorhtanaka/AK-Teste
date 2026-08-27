namespace WeatherDashboard.Api.Controllers;

using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using WeatherDashboard.Application.DTOs;
using WeatherDashboard.Application.Interfaces;

[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
public class WeatherController : ControllerBase
{
    private readonly IWeatherService _weatherService;
    private readonly IWeatherSyncService _weatherSyncService;
    private readonly ILogger<WeatherController> _logger;

    public WeatherController(
        IWeatherService weatherService,
        IWeatherSyncService weatherSyncService,
        ILogger<WeatherController> logger)
    {
        _weatherService = weatherService;
        _weatherSyncService = weatherSyncService;
        _logger = logger;
    }

    /// <summary>
    /// Retorna a leitura climática mais recente de uma capital.
    /// </summary>
    [HttpGet("current")]
    [ProducesResponseType(typeof(WeatherReadingDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<WeatherReadingDto>> GetCurrent([FromQuery] int cityId, CancellationToken cancellationToken)
    {
        var result = await _weatherService.GetCurrentWeatherAsync(cityId, cancellationToken);
        if (result == null)
        {
            return NotFound(new { message = $"Nenhuma leitura climática encontrada para a capital com ID {cityId}." });
        }
        return Ok(result);
    }

    /// <summary>
    /// Retorna o histórico de leituras de uma capital em um intervalo de datas.
    /// </summary>
    [HttpGet("history")]
    [ProducesResponseType(typeof(WeatherHistoryResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<WeatherHistoryResponseDto>> GetHistory([FromQuery] WeatherHistoryFilterDto filter, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _weatherService.GetWeatherHistoryAsync(filter, cancellationToken);
            return Ok(result);
        }
        catch (ValidationException ex)
        {
            return BadRequest(new
            {
                message = "Parâmetros de filtro inválidos.",
                errors = ex.Errors.Select(e => e.ErrorMessage)
            });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Retorna as estatísticas consolidadas do dia atual para uma capital.
    /// </summary>
    [HttpGet("stats/today")]
    [ProducesResponseType(typeof(TodayStatsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TodayStatsDto>> GetTodayStats([FromQuery] int cityId, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _weatherService.GetTodayStatsAsync(cityId, cancellationToken);
            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Retorna o panorama consolidado nacional (capital mais quente, mais fria e médias nacionais).
    /// </summary>
    [HttpGet("summary/national")]
    [ProducesResponseType(typeof(NationalSummaryDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<NationalSummaryDto>> GetNationalSummary(CancellationToken cancellationToken)
    {
        var result = await _weatherService.GetNationalSummaryAsync(cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Exporta o histórico climático filtrado no formato CSV com codificação UTF-8.
    /// </summary>
    [HttpGet("export")]
    [Produces("text/csv")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ExportCsv([FromQuery] WeatherHistoryFilterDto filter, CancellationToken cancellationToken)
    {
        try
        {
            var bytes = await _weatherService.ExportHistoryCsvAsync(filter, cancellationToken);
            var fileName = $"clima_capital_{filter.CityId}_{DateTime.UtcNow:yyyyMMdd_HHmmss}.csv";
            return File(bytes, "text/csv; charset=utf-8", fileName);
        }
        catch (ValidationException ex)
        {
            return BadRequest(new { errors = ex.Errors.Select(e => e.ErrorMessage) });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Dispara manualmente a coleta e sincronização climática de todas as capitais.
    /// </summary>
    [HttpPost("sync")]
    [ProducesResponseType(typeof(SyncResultDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<SyncResultDto>> TriggerSync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Disparo manual de sincronização climática recebido.");
        var result = await _weatherSyncService.SyncAllCitiesWeatherAsync(cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Retorna o status de conexão em tempo real e diagnósticos detalhados da integração com a OpenWeatherMap.
    /// </summary>
    [HttpGet("diagnostics")]
    [ProducesResponseType(typeof(ApiDiagnosticDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiDiagnosticDto>> GetDiagnostics(CancellationToken cancellationToken)
    {
        var diagnostics = await _weatherSyncService.GetLatestDiagnosticsAsync(cancellationToken);
        return Ok(diagnostics);
    }
}
