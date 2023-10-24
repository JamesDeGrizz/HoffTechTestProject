using Quartz;

namespace Application.Helpers;

public static class TriggersHelper
{
    public static readonly string TriggerIntervalKeyName = "interval";
    
    public static readonly string DailyTriggerValueName = "daily";
    public static readonly string MinutedTriggerValueName = "minuted";
    public static readonly string OnceTriggerValueName = "once";

    public static ITrigger GetMinutedTriggerUntilDayEnd(string jobKeyName, string triggerKeyName, int interval)
    {
        return TriggerBuilder.Create()
            .ForJob(jobKeyName)
            .WithIdentity(triggerKeyName)
            .StartNow()
            .UsingJobData(TriggerIntervalKeyName, MinutedTriggerValueName)
            .WithSimpleSchedule(x => x
                .WithIntervalInMinutes(interval)
                .RepeatForever()
            )
            .EndAt(DateBuilder.DateOf(23, 50, 0))
            .Build();
    }

    public static ITrigger GetDailyTriggerForTomorrowExchangeRate(string jobKeyName, string triggerKeyName)
    {
        return TriggerBuilder.Create()
            .ForJob(jobKeyName)
            .WithIdentity(triggerKeyName)
            .StartNow()
            .UsingJobData(TriggerIntervalKeyName, DailyTriggerValueName)
            .WithSchedule(CronScheduleBuilder.AtHourAndMinuteOnGivenDaysOfWeek(11, 30,
                               new DayOfWeek[] { DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday }))
            .Build();
    }
}
