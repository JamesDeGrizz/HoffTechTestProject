using Application.Models;

namespace Application.Services;

public interface IExchangeRatesService
{
    Task<bool> InitializeRatesAsync();

    void MoveRates();

    Task<bool> UpdateTomorrowExchangeRateAsync();

    Task<decimal> GetExchangeRateAsync(RequestDayEnum day);
}
