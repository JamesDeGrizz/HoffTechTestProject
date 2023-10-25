using Application.UseCases.ExchangeRates.GetExchangeRateForDay;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers;

[Route("api/v1/exchange-rates")]
[ApiController]
public class ExchangeRatesController : ControllerBase
{
    private readonly IMediator _mediator;

    public ExchangeRatesController(IMediator mediator)
     => _mediator = mediator;

    [HttpGet("to-ruble")]
    [ProducesResponseType(typeof(GetExchangeRateForDayResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> RubleConversionRate(double x, double y) 
        => Ok(await _mediator.Send(new GetExchangeRateForDayQuery(x, y)));
}
