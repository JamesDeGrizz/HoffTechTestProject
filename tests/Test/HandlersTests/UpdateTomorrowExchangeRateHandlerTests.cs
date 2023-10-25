using Application.Repositories;
using Application.UseCases.ExchangeRates.UpdateTomorrowExchangeRate;
using Integrations.Cbr;
using Microsoft.Extensions.Configuration;
using Moq;

namespace TestApplication.HandlersTests;

public class UpdateTomorrowExchangeRateHandlerTests
{
    [Fact]
    public void Ctor()
    {
        var inMemorySettings = new Dictionary<string, string> {
            {"Internal:CurrencyCode", "123"}
        };

        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        var repoMock = new Mock<IExchangeRatesRepository>();
        var cbrClientMock = new Mock<DailyInfoSoap>();

        new UpdateTomorrowExchangeRateHandler(cbrClientMock.Object, repoMock.Object, configuration);

        Assert.Throws<ArgumentException>(() => new UpdateTomorrowExchangeRateHandler(null, repoMock.Object, configuration));
        Assert.Throws<ArgumentException>(() => new UpdateTomorrowExchangeRateHandler(cbrClientMock.Object, null, configuration));
        Assert.Throws<ArgumentException>(() => new UpdateTomorrowExchangeRateHandler(cbrClientMock.Object, repoMock.Object, null));

        configuration = new ConfigurationBuilder().Build();
        Assert.Throws<InvalidOperationException>(() => new UpdateTomorrowExchangeRateHandler(cbrClientMock.Object, repoMock.Object, configuration));
    }

    // TODO: finish test
    //[Fact]
    //public async Task Handle()
    //{
    //    var inMemorySettings = new Dictionary<string, string> {
    //        {"Internal:CurrencyCode", "123"}
    //    };

    //    IConfiguration configuration = new ConfigurationBuilder()
    //        .AddInMemoryCollection(inMemorySettings)
    //        .Build();

    //    var repoMock = new Mock<IExchangeRatesRepository>();
    //    var cbrClientMock = new Mock<DailyInfoSoap>();

    //    var handler = new InitializeRatesHandler(cbrClientMock.Object, repoMock.Object, configuration);
    //    await handler.Handle(new InitializeRatesQuery(), CancellationToken.None);
    //}
}
