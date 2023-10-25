using Application.Models;
using MediatR;
using System.Text.Json.Serialization;

namespace Application.UseCases.ExchangeRates.GetExchangeRateForDay;

public class GetExchangeRateForDayResult : IRequest<GetExchangeRateForDayResult>
{
    public decimal Rate { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public RequestDayEnum Date { get; set; }
}
