using Application.Models;
using Application.UseCases.ExchangeRates.GetExchangeRateForDay;

namespace Application.Helpers;

public static class CircleHelper
{
    public static bool IsEntry(GetExchangeRateForDayQuery point, double radius)
    {
        if (point is null)
        {
            throw new ArgumentException(nameof(point));
        }
        return Math.Pow(point.X, 2) + Math.Pow(point.Y, 2) <= Math.Pow(radius, 2);
    }

    public static RequestDayEnum GetDayByQuarter(GetExchangeRateForDayQuery point)
    {
        if (point is null)
        {
            throw new ArgumentException(nameof(point));
        }

        return (point.X, point.Y) switch
        {
            ( >= 0, >= 0) => RequestDayEnum.Today,
            ( >= 0, < 0) => RequestDayEnum.Tomorrow,
            ( < 0, >= 0) => RequestDayEnum.Yesterday,
            ( < 0, < 0) => RequestDayEnum.DayBeforeYesterday,
            _ => throw new ArgumentException(nameof(point)),
        };
    }
}
