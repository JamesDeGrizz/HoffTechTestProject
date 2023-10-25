using Application.Exceptions;
using Application.Helpers;
using Application.UseCases.ExchangeRates.UpdateTomorrowExchangeRate;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Quartz;

namespace Application.Jobs;

public class TomorrowRateJob : IJob
{
    private readonly ILogger<TomorrowRateJob> _logger;
    private readonly IMediator _mediator;
    private readonly int _intervalMinutes;

    public TomorrowRateJob(ILogger<TomorrowRateJob> logger, IConfiguration configuration, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;

        _intervalMinutes = configuration.GetValue<int>("Internal:RetryIntervalMinutes");
    }

    public async Task Execute(IJobExecutionContext context)
    {
        _logger.LogTrace($"{nameof(TomorrowRateJob)} started");
        
        bool updated = false;
        try
        {
            updated = await _mediator.Send(new UpdateTomorrowExchangeRateQuery());
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Unknown exception");
        }

        if (!updated)
        {
            string state = context.MergedJobDataMap.GetString(TriggersHelper.TriggerIntervalKeyName);
            if (state == TriggersHelper.DailyTriggerValueName)
            {
                var key = new TriggerKey(context.Trigger.Key.Name);
                var trigger = TriggersHelper.GetMinutedTriggerUntilDayEnd(context.JobDetail.Key.Name, context.Trigger.Key.Name, _intervalMinutes);

                await context.Scheduler.RescheduleJob(key, trigger);
            }

            if (context.NextFireTimeUtc is null)
            {
                await RescheduleByDailyTrigger(context);
            }
        }
        else
        {
            string state = context.MergedJobDataMap.GetString(TriggersHelper.TriggerIntervalKeyName);
            if (state == TriggersHelper.MinutedTriggerValueName)
            {
                await RescheduleByDailyTrigger(context);
            }
        }
    }

    private async Task RescheduleByDailyTrigger(IJobExecutionContext context)
    {
        var key = new TriggerKey(context.Trigger.Key.Name);
        var trigger = TriggersHelper.GetDailyTriggerForTomorrowExchangeRate(context.JobDetail.Key.Name, context.Trigger.Key.Name);

        await context.Scheduler.RescheduleJob(key, trigger);
    }
}
