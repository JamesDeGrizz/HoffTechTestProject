using MediatR;

namespace Application.UseCases.ExchangeRates.GetExchangeRateForDay;

public class GetExchangeRateForDayQuery : IRequest<GetExchangeRateForDayResult>
{
    public GetExchangeRateForDayQuery(double x, double y)
        => (X, Y) = (x, y);

    public double X { get; set; }
    public double Y { get; set; }
}
