using Application.Helpers;
using Application.UseCases.ExchangeRates.InitializeRates;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Quartz;

namespace Application.Jobs;

public class InitializeRatesJob : IJob
{
    private readonly ILogger<InitializeRatesJob> _logger;
    private readonly IMediator _mediator;

    private readonly int _intervalMinutes;

    public InitializeRatesJob(ILogger<InitializeRatesJob> logger, IConfiguration configuration, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;

        _intervalMinutes = configuration.GetValue<int>("Internal:RetryIntervalMinutes");
    }

    public async Task Execute(IJobExecutionContext context)
    {
        _logger.LogTrace($"{nameof(InitializeRatesJob)} started");
        bool initialized = false;
        try
        {
            initialized = await _mediator.Send(new InitializeRatesQuery());
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Unknown exception");
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
