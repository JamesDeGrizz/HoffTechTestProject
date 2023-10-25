using Application.Exceptions;
using Application.UseCases.ExchangeRates.MoveRates;
using MediatR;
using Microsoft.Extensions.Logging;
using Quartz;

namespace Application.Jobs;

public class MoveRatesJob : IJob
{
    private readonly ILogger<MoveRatesJob> _logger;
    private readonly IMediator _mediator;

    public MoveRatesJob(ILogger<MoveRatesJob> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        _logger.LogTrace($"{nameof(MoveRatesJob)} started");
        
        try
        {
            await _mediator.Send(new MoveRatesQuery());
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Unknown exception");
        }
    }
}
