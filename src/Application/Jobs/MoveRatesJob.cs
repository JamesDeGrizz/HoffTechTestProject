using Application.Exceptions;
using Application.Services;
using Microsoft.Extensions.Logging;
using Quartz;

namespace Application.Jobs;

public class MoveRatesJob : IJob
{
    private readonly ILogger<MoveRatesJob> _logger;
    private readonly IExchangeRatesService _exchangeRatesService;
    
    public MoveRatesJob(ILogger<MoveRatesJob> logger, IExchangeRatesService exchangeRatesService)
    {
        _logger = logger;
        _exchangeRatesService = exchangeRatesService;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        _logger.LogTrace($"{nameof(MoveRatesJob)} started");
        
        try
        {
            _exchangeRatesService.MoveRates();
        }
        catch (NotInitializedException e)
        {
            _logger.LogError("Exchange rates repo is not initialized", e);
        }
        catch (Exception e)
        {
            _logger.LogError("Unknown exception", e);
        }
    }
}
