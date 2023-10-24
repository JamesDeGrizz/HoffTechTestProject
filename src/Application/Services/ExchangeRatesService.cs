using Application.Exceptions;
using Application.Models;
using Application.Repositories;
using Integrations.Cbr;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Xml.Serialization;

namespace Application.Services;

public class ExchangeRatesService : IExchangeRatesService
{
    private readonly ILogger<ExchangeRatesService> _logger;
    private readonly DailyInfoSoap _cbrClient;
    private readonly IExchangeRatesRepository _ratesRepository;

    private readonly XmlSerializer _xmlSerializer;
    private readonly ushort _currencyCode;
    private bool _initialized;

    public ExchangeRatesService(ILogger<ExchangeRatesService> logger, DailyInfoSoap cbrClient,
        IConfiguration configuration, IExchangeRatesRepository ratesRepository)
    {
        _logger = logger;
        _cbrClient = cbrClient;
        _ratesRepository = ratesRepository;

        _currencyCode = configuration.GetValue<ushort>("Internal:CurrencyCode");
        _xmlSerializer = new XmlSerializer(typeof(diffgram));
    }

    public async Task<bool> InitializeRatesAsync()
    {
        if (_initialized)
        {
            return true;
        }

        var dayBeforeYesterdayExchangeRate = await GetRatesOnDateAsync(DateTime.Now.AddDays(-2));
        var yesterdayExchangeRate = await GetRatesOnDateAsync(DateTime.Now.AddDays(-1));
        var todayExchangeRate = await GetRatesOnDateAsync(DateTime.Now);

        UpdateRateForDay(dayBeforeYesterdayExchangeRate, RequestDayEnum.DayBeforeYesterday);
        UpdateRateForDay(yesterdayExchangeRate, RequestDayEnum.Yesterday);
        UpdateRateForDay(todayExchangeRate, RequestDayEnum.Today);

        if (await IsUpdatedForTomorrow())
        {
            var tomorrowRate = await GetRatesOnDateAsync(DateTime.Now.AddDays(1));
            UpdateRateForDay(tomorrowRate, RequestDayEnum.Tomorrow);
        }

        _initialized = true;
        return true;
    }

    public async Task<decimal> GetExchangeRateAsync(RequestDayEnum day)
    {
        if (!_initialized)
        {
            throw new NotInitializedException();
        }

        var rate = _ratesRepository.GetExchangeRate(day);
        if (rate is null)
        {
            throw new NoValueException();
        }

        return rate.Value;
    }

    public async Task<bool> UpdateTomorrowExchangeRateAsync()
    {
        if (!_initialized)
        {
            throw new NotInitializedException();
        }

        if (!await IsUpdatedForTomorrow())
        {
            return false;
        }

        var rates = await GetRatesOnDateAsync(DateTime.Now.AddDays(1));
        UpdateRateForDay(rates, RequestDayEnum.Tomorrow);

        return true;
    }

    public void MoveRates()
    {
        if (!_initialized)
        {
            throw new NotInitializedException();
        }

        _ratesRepository.MoveExchangeRates();
    }

    private async Task<bool> IsUpdatedForTomorrow()
    {
        var latestUpdate = await _cbrClient.GetLatestDateTimeAsync();
        return latestUpdate.Date == DateTime.Now.Date;
    }

    private async Task<ValuteDataValuteCursOnDate[]> GetRatesOnDateAsync(DateTime date)
    {
        var ratesAndSchema = await _cbrClient.GetCursOnDateAsync(date);
        var ratesXml = ratesAndSchema.Nodes[1];
        var wholeXml = (diffgram)_xmlSerializer.Deserialize(ratesXml.CreateReader());
        return wholeXml.ValuteData;
    }

    private void UpdateRateForDay(ValuteDataValuteCursOnDate[] rates, RequestDayEnum day)
    {
        var tomorrowRate = rates
            .First(x => x.Vcode == _currencyCode);

        _ratesRepository.SetExchangeRate(day, tomorrowRate.Vcurs);
    }
}
