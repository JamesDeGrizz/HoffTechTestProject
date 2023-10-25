using Application.Models;
using Application.Repositories;

namespace TestApplication;

public class ExchangeRatesRepositoryTests
{
    [Fact]
    public void ExchangeRatesRepository_Ctor()
    {
        var repo = new ExchangeRatesRepository();
        
        Assert.Null(repo.GetExchangeRate(RequestDayEnum.DayBeforeYesterday));
        Assert.Null(repo.GetExchangeRate(RequestDayEnum.Yesterday));
        Assert.Null(repo.GetExchangeRate(RequestDayEnum.Today));
        Assert.Null(repo.GetExchangeRate(RequestDayEnum.Tomorrow));
    }

    [Theory]
    [InlineData(RequestDayEnum.DayBeforeYesterday, 123)]
    [InlineData(RequestDayEnum.Yesterday, 123)]
    [InlineData(RequestDayEnum.Today, 123)]
    [InlineData(RequestDayEnum.Tomorrow, 123)]
    public void SetExchangeRate(RequestDayEnum day, decimal value)
    {
        var repo = new ExchangeRatesRepository();
        
        repo.SetExchangeRate(day, value);
        Assert.Equal(value, repo.GetExchangeRate(day));

        repo.SetExchangeRate(day, null);
        Assert.Null(repo.GetExchangeRate(day));
    }

    [Fact]
    public void MoveExchangeRates()
    {
        var repo = new ExchangeRatesRepository();

        var dayBeforeYesterdayValue = 12M;
        var YesterdayValue = 23M;
        var TodayValue = 34M;
        var TomorrowValue = 45M;
        repo.SetExchangeRate(RequestDayEnum.DayBeforeYesterday, dayBeforeYesterdayValue);
        repo.SetExchangeRate(RequestDayEnum.Yesterday, YesterdayValue);
        repo.SetExchangeRate(RequestDayEnum.Today, TodayValue);
        repo.SetExchangeRate(RequestDayEnum.Tomorrow, TomorrowValue);

        repo.MoveExchangeRates();

        Assert.Equal(YesterdayValue, repo.GetExchangeRate(RequestDayEnum.DayBeforeYesterday));
        Assert.Equal(TodayValue, repo.GetExchangeRate(RequestDayEnum.Yesterday));
        Assert.Equal(TomorrowValue, repo.GetExchangeRate(RequestDayEnum.Today));
        Assert.Null(repo.GetExchangeRate(RequestDayEnum.Tomorrow));
    }

    [Fact]
    public void InitializeExchangeRates()
    {
        var repo = new ExchangeRatesRepository();

        var dayBeforeYesterdayValue = 12M;
        var YesterdayValue = 23M;
        var TodayValue = 34M;
        var TomorrowValue = 45M;
        repo.InitializeExchangeRates(dayBeforeYesterdayValue, YesterdayValue, TodayValue, TomorrowValue);

        Assert.Equal(dayBeforeYesterdayValue, repo.GetExchangeRate(RequestDayEnum.DayBeforeYesterday));
        Assert.Equal(YesterdayValue, repo.GetExchangeRate(RequestDayEnum.Yesterday));
        Assert.Equal(TodayValue, repo.GetExchangeRate(RequestDayEnum.Today));
        Assert.Equal(TomorrowValue, repo.GetExchangeRate(RequestDayEnum.Tomorrow));
    }
}