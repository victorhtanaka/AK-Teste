namespace WeatherDashboard.Api.Controllers;

using Microsoft.AspNetCore.Mvc;
using WeatherDashboard.Application.DTOs;
using WeatherDashboard.Application.Interfaces;

[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
public class CitiesController : ControllerBase
{
    private readonly ICityService _cityService;

    public CitiesController(ICityService cityService)
    {
        _cityService = cityService;
    }

    /// <summary>
    /// Retorna a lista de todas as 27 capitais brasileiras.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<CityDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<CityDto>>> GetAll(CancellationToken cancellationToken)
    {
        var cities = await _cityService.GetAllCitiesAsync(cancellationToken);
        return Ok(cities);
    }

    /// <summary>
    /// Retorna os dados de uma capital específica pelo seu ID.
    /// </summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(CityDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CityDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var city = await _cityService.GetCityByIdAsync(id, cancellationToken);
        if (city == null)
        {
            return NotFound(new { message = $"Capital com ID {id} não encontrada." });
        }
        return Ok(city);
    }
}
