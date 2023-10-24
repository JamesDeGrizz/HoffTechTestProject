using Application.Exceptions;
using Application.Helpers;
using Application.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Quartz;

namespace Application.Jobs;

public class InitializeRatesJob : IJob
{
    private readonly ILogger<InitializeRatesJob> _logger;
    private readonly IExchangeRatesService _exchangeRatesService;
    private readonly int _intervalMinutes;
    
    public InitializeRatesJob(ILogger<InitializeRatesJob> logger, IExchangeRatesService exchangeRatesService, IConfiguration configuration)
    {
        _logger = logger;
        _exchangeRatesService = exchangeRatesService;

        _intervalMinutes = configuration.GetValue<int>("Internal:RetryIntervalMinutes");
    }

    public async Task Execute(IJobExecutionContext context)
    {
        _logger.LogTrace($"{nameof(InitializeRatesJob)} started");
        bool initialized = false;
        try
        {
            initialized = await _exchangeRatesService.InitializeRatesAsync();
        }
        catch (NotInitializedException e)
        {
            _logger.LogError("Exchange rates repo is not initialized", e);
        }
        catch (Exception e)
        {
            _logger.LogError("Unknown exception", e);
        }

        if (!initialized)
        {
            string state = context.MergedJobDataMap.GetString(TriggersHelper.TriggerIntervalKeyName);
            if (state == TriggersHelper.OnceTriggerValueName)
            {
                var key = new TriggerKey(context.Trigger.Key.Name);
                var trigger = TriggersHelper.GetMinutedTriggerUntilDayEnd(context.JobDetail.Key.Name, context.Trigger.Key.Name, _intervalMinutes);

                await context.Scheduler.RescheduleJob(key, trigger);
            }
        }
        else
        {
            await context.Scheduler.UnscheduleJob(context.Trigger.Key);
        }
    }
}
