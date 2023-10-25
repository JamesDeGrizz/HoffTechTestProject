using Application.Extensions;
using Application.Repositories;
using Integrations.Cbr;
using MediatR;
using Microsoft.Extensions.Configuration;

namespace Application.UseCases.ExchangeRates.InitializeRates;

public class InitializeRatesHandler : IRequestHandler<InitializeRatesQuery, bool>
{
    private readonly DailyInfoSoap _cbrClient;
    private readonly IExchangeRatesRepository _ratesRepository;

    private readonly ushort _currencyCode;

    public InitializeRatesHandler(DailyInfoSoap cbrClient, IExchangeRatesRepository ratesRepository,
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

    public async Task<bool> Handle(InitializeRatesQuery request, CancellationToken cancellationToken)
    {
        var dayBeforeYesterdayExchangeRates = await _cbrClient.GetRatesOnDateAsync(DateTime.Now.AddDays(-2));
        var dayBeforeYesterdayExchangeRate = dayBeforeYesterdayExchangeRates.First(x => x.Vcode == _currencyCode).Vcurs;

        var yesterdayExchangeRates = await _cbrClient.GetRatesOnDateAsync(DateTime.Now.AddDays(-1));
        var yesterdayExchangeRate = yesterdayExchangeRates.First(x => x.Vcode == _currencyCode).Vcurs;

        var todayExchangeRates = await _cbrClient.GetRatesOnDateAsync(DateTime.Now);
        var todayExchangeRate = todayExchangeRates.First(x => x.Vcode == _currencyCode).Vcurs;

        decimal? tomorrowExchangeRate = null;

        if (await _cbrClient.IsUpdatedForTomorrow())
        {
            var tomorrowExchangeRates = await _cbrClient.GetRatesOnDateAsync(DateTime.Now.AddDays(1));
            tomorrowExchangeRate = tomorrowExchangeRates.First(x => x.Vcode == _currencyCode).Vcurs;
        }

        _ratesRepository.InitializeExchangeRates(dayBeforeYesterdayExchangeRate, yesterdayExchangeRate, todayExchangeRate, tomorrowExchangeRate);

        return true;
    }
}
