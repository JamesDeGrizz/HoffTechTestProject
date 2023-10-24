using Application.Exceptions;
using Application.Helpers;
using Application.Models;
using Application.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers;

[Route("api/v1/exchange-rates")]
[ApiController]
public class ExchangeRatesController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly IExchangeRatesService _exchangeRatesService;

    public ExchangeRatesController(IConfiguration configuration, IExchangeRatesService exchangeRatesService)
    { 
        _configuration = configuration;
        _exchangeRatesService = exchangeRatesService;
    }

    [HttpGet("to-ruble")]
    [ProducesResponseType(typeof(RubleConversionRateResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> RubleConversionRate(double x, double y) 
    {
        var point = new Point(x, y);
        var radius = _configuration.GetValue<double>("Internal:CircleRadius");
        if (!CircleHelper.IsEntry(point, radius))
        {
            return BadRequest($"Point [{x}, {y}] is not in circle with radius {radius}");
        }
        var requestDay = CircleHelper.GetDayByQuarter(point);

        try
        {
            var rate = await _exchangeRatesService.GetExchangeRateAsync(requestDay);

            return Ok(new RubleConversionRateResult
            {
                Rate = 1 / rate,
                Date = requestDay
            });
        }
        catch (NoValueException)
        {
            return BadRequest($"Don't have exchange rate for requested day");
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, $"Technical issues on server, please try again later");
        }
    }
}
