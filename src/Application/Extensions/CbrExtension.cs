using Integrations.Cbr;
using System.Xml.Serialization;

namespace Application.Extensions;

public static class CbrExtension
{
    private static readonly XmlSerializer _xmlSerializer = new XmlSerializer(typeof(diffgram));

    public static async Task<bool> IsUpdatedForTomorrow(this DailyInfoSoap cbrClient)
    {
        var latestUpdate = await cbrClient.GetLatestDateTimeAsync();
        return latestUpdate.Date == DateTime.Now.Date;
    }

    public static async Task<ValuteDataValuteCursOnDate[]> GetRatesOnDateAsync(this DailyInfoSoap cbrClient, DateTime date)
    {
        var ratesAndSchema = await cbrClient.GetCursOnDateAsync(date);
        var ratesXml = ratesAndSchema.Nodes[1];
        var wholeXml = (diffgram)_xmlSerializer.Deserialize(ratesXml.CreateReader());
        return wholeXml.ValuteData;
    }
}
