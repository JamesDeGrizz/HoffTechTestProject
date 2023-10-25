using Application.Exceptions;
using Application.Helpers;
using Application.Repositories;
using MediatR;
using Microsoft.Extensions.Configuration;

namespace Application.UseCases.ExchangeRates.GetExchangeRateForDay;

public class GetExchangeRateForDayHandler : IRequestHandler<GetExchangeRateForDayQuery, GetExchangeRateForDayResult>
{
    private readonly IExchangeRatesRepository _ratesRepository;

    private readonly double _radius;

    public GetExchangeRateForDayHandler(IConfiguration configuration, IExchangeRatesRepository ratesRepository)
    {
        if (configuration is null)
        {
            throw new ArgumentException(nameof(configuration));
        }
        if (ratesRepository is null)
        {
            throw new ArgumentException(nameof(ratesRepository));
        }

        _ratesRepository = ratesRepository;

        var internalSection = configuration.GetRequiredSection("Internal");
        _radius = internalSection.GetValue<double>("CircleRadius");
    }
        
    public Task<GetExchangeRateForDayResult> Handle(GetExchangeRateForDayQuery request, CancellationToken cancellationToken)
    {
        if (!CircleHelper.IsEntry(request, _radius))
        {
            throw new NotInCircleException(request.X, request.Y, _radius);
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
