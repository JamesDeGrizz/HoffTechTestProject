using Application.Extensions;
using Application.Models;
using Application.Repositories;
using Integrations.Cbr;
using MediatR;
using Microsoft.Extensions.Configuration;

namespace Application.UseCases.ExchangeRates.UpdateTomorrowExchangeRate;

public class UpdateTomorrowExchangeRateHandler : IRequestHandler<UpdateTomorrowExchangeRateQuery, bool>
{
    private readonly DailyInfoSoap _cbrClient;
    private readonly IExchangeRatesRepository _ratesRepository;

    private readonly ushort _currencyCode;

    public UpdateTomorrowExchangeRateHandler(DailyInfoSoap cbrClient, IExchangeRatesRepository ratesRepository,
        IConfiguration configuration)
    {
        if (cbrClient is null)
        {
            throw new ArgumentException(nameof(cbrClient));
        }
        if (configuration is null)
        {
            throw new ArgumentException(nameof(configuration));
        }
        if (ratesRepository is null)
        {
            throw new ArgumentException(nameof(ratesRepository));
        }

        _cbrClient = cbrClient;
        _ratesRepository = ratesRepository;

        var internalSection = configuration.GetRequiredSection("Internal");
        _currencyCode = internalSection.GetValue<ushort>("CurrencyCode");
    }

    public async Task<bool> Handle(UpdateTomorrowExchangeRateQuery request, CancellationToken cancellationToken)
    {
        if (!await _cbrClient.IsUpdatedForTomorrow())
        {
            return false;
        }

        var rates = await _cbrClient.GetRatesOnDateAsync(DateTime.Now.AddDays(1));
        var tomorrowRate = rates
            .First(x => x.Vcode == _currencyCode);

        _ratesRepository.SetExchangeRate(RequestDayEnum.Tomorrow, tomorrowRate.Vcurs);

        return true;
    }
}
