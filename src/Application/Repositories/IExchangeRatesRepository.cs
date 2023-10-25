using Application.Models;

namespace Application.Repositories;

public interface IExchangeRatesRepository
{
    void InitializeExchangeRates(decimal dayBeforeYesterdayRate, decimal yesterdayRate, decimal todayRate, decimal? tomorrowRate);
    decimal? GetExchangeRate(RequestDayEnum day);
    void SetExchangeRate(RequestDayEnum day, decimal? value);
    void MoveExchangeRates();
}
