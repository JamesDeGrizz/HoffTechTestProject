using Application.Exceptions;
using Application.Models;
using Application.Repositories;
using Application.UseCases.ExchangeRates.GetExchangeRateForDay;
using Microsoft.Extensions.Configuration;
using Moq;

namespace TestApplication.HandlersTests;

public class GetExchangeRateForDayHandlerTests
{
    [Fact]
    public void Ctor()
    {
        var inMemorySettings = new Dictionary<string, string> {
            {"Internal:CircleRadius", "5"}
        };

        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        var repoMock = new Mock<IExchangeRatesRepository>();

        new GetExchangeRateForDayHandler(configuration, repoMock.Object);

        Assert.Throws<ArgumentException>(() => new GetExchangeRateForDayHandler(null, repoMock.Object));
        Assert.Throws<ArgumentException>(() => new GetExchangeRateForDayHandler(configuration, null));

        configuration = new ConfigurationBuilder().Build();
        Assert.Throws<InvalidOperationException>(() => new GetExchangeRateForDayHandler(configuration, repoMock.Object));
    }

    [Theory]
    [InlineData(RequestDayEnum.DayBeforeYesterday, 5, -3, -3)]
    [InlineData(RequestDayEnum.Yesterday, 6, -3, 3)]
    [InlineData(RequestDayEnum.Today, 7, 3, 3)]
    [InlineData(RequestDayEnum.Tomorrow, 8, 3, -3)]
    public async Task Handle_CorrectInput(RequestDayEnum day, decimal rate, double x, double y)
    {
        var inMemorySettings = new Dictionary<string, string> {
            {"Internal:CircleRadius", "5"}
        };

        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        var repoMock = new Mock<IExchangeRatesRepository>();
        var handler = new GetExchangeRateForDayHandler(configuration, repoMock.Object);

        repoMock.Setup(x => x.GetExchangeRate(day)).Returns(rate);
        var result = await handler.Handle(new GetExchangeRateForDayQuery(x, y), CancellationToken.None);
        Assert.Equal(1 / rate, result.Rate);
        Assert.Equal(day, result.Date);
    }

    [Fact]
    public async Task Handle_NotInCircleException()
    {
        var inMemorySettings = new Dictionary<string, string> {
            {"Internal:CircleRadius", "5"}
        };

        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        var repoMock = new Mock<IExchangeRatesRepository>();
        var handler = new GetExchangeRateForDayHandler(configuration, repoMock.Object);
        await Assert.ThrowsAsync<NotInCircleException>(async () =>
            await handler.Handle(new GetExchangeRateForDayQuery(5, 5), CancellationToken.None));
    }

    [Fact]
    public async Task Handle_NoValueException()
    {
        var inMemorySettings = new Dictionary<string, string> {
            {"Internal:CircleRadius", "5"}
        };

        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        var repoMock = new Mock<IExchangeRatesRepository>();
        repoMock.Setup(x => x.GetExchangeRate(RequestDayEnum.Today)).Returns((decimal?)null);
        var handler = new GetExchangeRateForDayHandler(configuration, repoMock.Object);
        await Assert.ThrowsAsync<NoValueException>(async () =>
            await handler.Handle(new GetExchangeRateForDayQuery(3, 3), CancellationToken.None));
    }
}
