namespace CurrencyConverter.Core.Entities
{
    public class ExchangeRate
    {
        public string BaseCurrency { get; set; }
        public DateTime Date { get; set; }
        public Dictionary<string, decimal> Rates { get; set; } = new Dictionary<string, decimal>();
    }
}
