using Application.Models;
using System.Collections.Concurrent;

namespace Application.Repositories;

public class ExchangeRatesRepository : IExchangeRatesRepository
{
    private readonly ConcurrentDictionary<RequestDayEnum, decimal?> _exchangeRates = new ConcurrentDictionary<RequestDayEnum, decimal?>();

    public ExchangeRatesRepository() 
    {
        _exchangeRates.TryAdd(RequestDayEnum.DayBeforeYesterday, null);
        _exchangeRates.TryAdd(RequestDayEnum.Yesterday, null);
        _exchangeRates.TryAdd(RequestDayEnum.Today, null);
        _exchangeRates.TryAdd(RequestDayEnum.Tomorrow, null);
    }

    public decimal? GetExchangeRate(RequestDayEnum day)
        => _exchangeRates[day];

    public void SetExchangeRate(RequestDayEnum day, decimal? value)
        => _exchangeRates[day] = value;

    public void MoveExchangeRates()
    {
        _exchangeRates[RequestDayEnum.DayBeforeYesterday] = _exchangeRates[RequestDayEnum.Yesterday];
        _exchangeRates[RequestDayEnum.Yesterday] = _exchangeRates[RequestDayEnum.Today];
        _exchangeRates[RequestDayEnum.Today] = _exchangeRates[RequestDayEnum.Tomorrow];
        _exchangeRates[RequestDayEnum.Tomorrow] = null;
    }

    public void InitializeExchangeRates(decimal dayBeforeYesterdayRate, decimal yesterdayRate, decimal todayRate, decimal? tomorrowRate)
    {
        _exchangeRates[RequestDayEnum.DayBeforeYesterday] = dayBeforeYesterdayRate;
        _exchangeRates[RequestDayEnum.Yesterday] = yesterdayRate;
        _exchangeRates[RequestDayEnum.Today] = todayRate;
        _exchangeRates[RequestDayEnum.Tomorrow] = tomorrowRate;
    }
}
