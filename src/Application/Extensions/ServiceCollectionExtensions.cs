using Application.Behaviours;
using Application.Helpers;
using Application.Jobs;
using Application.Repositories;
using Integrations.Cbr;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using static Integrations.Cbr.DailyInfoSoapClient;

namespace Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddQuartzJobs(this IServiceCollection services)
    {
        services.AddQuartz(q =>
        {
            q.ScheduleJob<TomorrowRateJob>(
                    triggerConfigurator => triggerConfigurator
                        .ForJob("TomorrowRateJob")
                        .WithIdentity("TomorrowRateJob")
                        .StartAt(DateTimeOffset.Now.AddSeconds(5))
                        .UsingJobData(TriggersHelper.TriggerIntervalKeyName, TriggersHelper.DailyTriggerValueName)
                        .WithSchedule(CronScheduleBuilder.AtHourAndMinuteOnGivenDaysOfWeek(11, 30,
                            new DayOfWeek[] { DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday }))
                        ,
                    jobConfigurator => jobConfigurator
                        .WithIdentity("TomorrowRateJob")
                        );

            q.ScheduleJob<MoveRatesJob>(
                    triggerConfigurator => triggerConfigurator
                        .ForJob("MoveRatesJob")
                        .WithIdentity("MoveRatesJob")
                        .StartAt(DateTimeOffset.Now.AddSeconds(5))
                        .UsingJobData(TriggersHelper.TriggerIntervalKeyName, TriggersHelper.DailyTriggerValueName)
                        .WithSchedule(CronScheduleBuilder.AtHourAndMinuteOnGivenDaysOfWeek(0, 0,
                            new DayOfWeek[] { DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday }))
                        ,
                    jobConfigurator => jobConfigurator
                        .WithIdentity("MoveRatesJob")
                        );

            q.ScheduleJob<InitializeRatesJob>(triggerConfigurator => triggerConfigurator
                        .ForJob("InitializeRatesJob")
                        .WithIdentity("InitializeRatesJob")
                        .UsingJobData(TriggersHelper.TriggerIntervalKeyName, TriggersHelper.OnceTriggerValueName)
                        .StartNow()
                        ,
                    jobConfigurator => jobConfigurator
                        .WithIdentity("InitializeRatesJob")
                        );
        });

        services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

        return services;
    }

    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IExchangeRatesRepository, ExchangeRatesRepository>();

        var cbrUrl = configuration.GetValue<string>("Internal:CbrUrl");
        services.AddSingleton<DailyInfoSoap>(x =>
            ActivatorUtilities.CreateInstance<DailyInfoSoapClient>(x, EndpointConfiguration.DailyInfoSoap12, cbrUrl)
        );

        return services;
    }

    public static IServiceCollection AddMediatr(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(ServiceCollectionExtensions).Assembly));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(UnhandledExceptionBehaviour<,>));
        return services;
    }
}
