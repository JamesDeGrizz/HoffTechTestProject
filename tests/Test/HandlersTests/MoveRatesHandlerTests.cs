using Application.Repositories;
using Application.UseCases.ExchangeRates.MoveRates;
using Moq;

namespace TestApplication.HandlersTests;

public class MoveRatesHandlerTests
{
    [Fact]
    public void Ctor()
    {
        var repoMock = new Mock<IExchangeRatesRepository>();

        _ = new MoveRatesHandler(repoMock.Object);

        Assert.Throws<ArgumentException>(() => _ = new MoveRatesHandler(null));
    }

    [Fact]
    public async Task Handle()
    {
        var repoMock = new Mock<IExchangeRatesRepository>();
        var handler = new MoveRatesHandler(repoMock.Object);
        await handler.Handle(new MoveRatesQuery(), CancellationToken.None);

        repoMock.Verify(x => x.MoveExchangeRates(), Times.Once());
    }
}
