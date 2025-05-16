namespace CurrencyConverter.Core.Entities
{
    public class ExchangeRateTimeSeries
    {
        public decimal Amount { get; set; }

        public string Base { get; set; }

        public DateOnly StartDate { get; set; }

        public DateOnly EndDate { get; set; }

        public Dictionary<DateOnly, Dictionary<string, decimal>> Rates { get; set; }
    }
}
