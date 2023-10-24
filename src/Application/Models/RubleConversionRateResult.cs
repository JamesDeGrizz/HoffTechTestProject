using System.Text.Json.Serialization;

namespace Application.Models;

public class RubleConversionRateResult
{
    public decimal Rate { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public RequestDayEnum Date { get; set; }
}
