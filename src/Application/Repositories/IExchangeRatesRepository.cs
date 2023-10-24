using Application.Models;

namespace Application.Repositories;

public interface IExchangeRatesRepository
{
    decimal? GetExchangeRate(RequestDayEnum day);
    void SetExchangeRate(RequestDayEnum day, decimal? value);
    void MoveExchangeRates();
}
