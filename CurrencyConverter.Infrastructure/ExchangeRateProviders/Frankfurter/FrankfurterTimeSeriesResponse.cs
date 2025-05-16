using System.Text.Json.Serialization;

namespace CurrencyConverter.Infrastructure.ExchangeRateProviders.Frankfurter
{
    internal class FrankfurterTimeSeriesResponse
    {
        [JsonPropertyName("amount")]
        public decimal Amount { get; set; }

        [JsonPropertyName("base")]
        public string Base { get; set; }

        [JsonPropertyName("start_date")]
        public DateOnly StartDate { get; set; }

        [JsonPropertyName("end_date")]
        public DateOnly EndDate { get; set; }

        [JsonPropertyName("rates")]
        public Dictionary<DateOnly, Dictionary<string, decimal>> Rates { get; set; }
    }
}
