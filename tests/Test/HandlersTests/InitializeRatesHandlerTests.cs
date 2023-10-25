using Application.Repositories;
using Application.UseCases.ExchangeRates.InitializeRates;
using Integrations.Cbr;
using Microsoft.Extensions.Configuration;
using Moq;

namespace TestApplication.HandlersTests;

public class InitializeRatesHandlerTests
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

        new InitializeRatesHandler(cbrClientMock.Object, repoMock.Object, configuration);

        Assert.Throws<ArgumentException>(() => new InitializeRatesHandler(null, repoMock.Object, configuration));
        Assert.Throws<ArgumentException>(() => new InitializeRatesHandler(cbrClientMock.Object, null, configuration));
        Assert.Throws<ArgumentException>(() => new InitializeRatesHandler(cbrClientMock.Object, repoMock.Object, null));

        configuration = new ConfigurationBuilder().Build();
        Assert.Throws<InvalidOperationException>(() => new InitializeRatesHandler(cbrClientMock.Object, repoMock.Object, configuration));
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
