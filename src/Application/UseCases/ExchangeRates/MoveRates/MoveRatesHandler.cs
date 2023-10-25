using Application.Repositories;
using MediatR;

namespace Application.UseCases.ExchangeRates.MoveRates;

public class MoveRatesHandler : IRequestHandler<MoveRatesQuery>
{
    private readonly IExchangeRatesRepository _ratesRepository;

    public MoveRatesHandler(IExchangeRatesRepository ratesRepository)
        => _ratesRepository = ratesRepository;

    public Task Handle(MoveRatesQuery request, CancellationToken cancellationToken)
    {
        _ratesRepository.MoveExchangeRates();
        return Task.CompletedTask;
    }
}
