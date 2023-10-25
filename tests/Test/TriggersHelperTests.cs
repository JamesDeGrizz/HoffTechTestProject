using Application.Helpers;

namespace TestApplication;

public class TriggersHelperTests
{
    [Fact]
    public void GetMinutedTriggerUntilDayEnd_CorrectInput()
    {
        var jobName = "JobName";
        var keyName = "KeyName";
        var interval = 10;
        var trigger = TriggersHelper.GetMinutedTriggerUntilDayEnd(jobName, keyName, interval);

        Assert.Equal(jobName, trigger.JobKey.Name);
        Assert.Equal(keyName, trigger.Key.Name);

        //TODO: fix it
        //The value returned is not guaranteed to be valid until after the has been added to the scheduler.
        //Assert.True(trigger.GetNextFireTimeUtc() <= DateTime.Now.AddMinutes(interval));

        var expectedTimeStr = "23:50";
        var expectedTime = DateTime.ParseExact(expectedTimeStr, "H:mm", null,
            System.Globalization.DateTimeStyles.AdjustToUniversal | System.Globalization.DateTimeStyles.AssumeLocal);
        Assert.True(trigger.FinalFireTimeUtc > expectedTime.AddMinutes(-10));
        Assert.True(expectedTime.AddMinutes(20) > trigger.FinalFireTimeUtc);
    }

    [Fact]
    public void GetDailyTriggerForTomorrowExchangeRate_CorrectInput()
    {
        var jobName = "JobName";
        var keyName = "KeyName";
        var trigger = TriggersHelper.GetDailyTriggerForTomorrowExchangeRate(jobName, keyName);

        Assert.Equal(jobName, trigger.JobKey.Name);
        Assert.Equal(keyName, trigger.Key.Name);
    }
}