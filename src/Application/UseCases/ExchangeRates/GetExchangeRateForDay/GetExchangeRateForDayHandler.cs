using Application.Exceptions;
using Application.Helpers;
using Application.Repositories;
using MediatR;
using Microsoft.Extensions.Configuration;

namespace Application.UseCases.ExchangeRates.GetExchangeRateForDay;

public class GetExchangeRateForDayHandler : IRequestHandler<GetExchangeRateForDayQuery, GetExchangeRateForDayResult>
{
    private readonly IConfiguration _configuration;
    private readonly IExchangeRatesRepository _ratesRepository;

    public GetExchangeRateForDayHandler(IConfiguration configuration, IExchangeRatesRepository ratesRepository)
        => (_configuration, _ratesRepository) = (configuration, ratesRepository);

    public Task<GetExchangeRateForDayResult> Handle(GetExchangeRateForDayQuery request, CancellationToken cancellationToken)
    {
        var radius = _configuration.GetValue<double>("Internal:CircleRadius");
        if (!CircleHelper.IsEntry(request, radius))
        {
            throw new NotInCircleException(request.X, request.Y, radius);
        }
        var requestDay = CircleHelper.GetDayByQuarter(request);

        var rate = _ratesRepository.GetExchangeRate(requestDay);
        if (rate is null)
        {
            throw new NoValueException();
        }

        return Task.FromResult(new GetExchangeRateForDayResult
        {
            Rate = 1 / rate.Value,
            Date = requestDay
        });
    }
}
