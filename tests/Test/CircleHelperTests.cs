using Application.Helpers;
using Application.Models;
using Application.UseCases.ExchangeRates.GetExchangeRateForDay;

namespace TestApplication;

public class CircleHelperTests
{
    [Theory]
    [InlineData(3, 3)]
    [InlineData(3, -3)]
    [InlineData(-3, 3)]
    [InlineData(-3, -3)]
    [InlineData(0, 0)]
    public void IsEntry_ShouldReturnTrue(double x, double y)
    {
        Assert.True(CircleHelper.IsEntry(new GetExchangeRateForDayQuery(3, 3), 5));
    }

    [Theory]
    [InlineData(3, 3)]
    [InlineData(3, -3)]
    [InlineData(-3, 3)]
    [InlineData(-3, -3)]
    public void IsEntry_ShouldReturnFalse(double x, double y)
    {
        Assert.False(CircleHelper.IsEntry(new GetExchangeRateForDayQuery(3, 3), 1));
    }

    [Fact]
    public void IsEntry_CheckBorder()
    {
        Assert.False(CircleHelper.IsEntry(new GetExchangeRateForDayQuery(1, 1), 1));
    }

    [Fact]
    public void IsEntry_NullPoint()
    {
        Assert.Throws<ArgumentException>(() => CircleHelper.IsEntry(null, 1));
    }

    [Theory]
    [InlineData(3, 3, RequestDayEnum.Today)]
    [InlineData(-3, 3, RequestDayEnum.Yesterday)]
    [InlineData(-3, -3, RequestDayEnum.DayBeforeYesterday)]
    [InlineData(3, -3, RequestDayEnum.Tomorrow)]
    public void GetDayByQuarter_CorrectInput(double x, double y, RequestDayEnum day)
    {
        Assert.Equal(day, CircleHelper.GetDayByQuarter(new GetExchangeRateForDayQuery(x, y)));
    }

    [Fact]
    public void GetDayByQuarter_IncorrectInput_ShouldThrowArgumentException()
    {
        Assert.Throws<ArgumentException>(() => CircleHelper.GetDayByQuarter(null));
    }
}