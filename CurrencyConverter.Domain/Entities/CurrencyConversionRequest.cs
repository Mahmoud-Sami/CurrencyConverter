namespace CurrencyConverter.Core.Entities
{
    public class CurrencyConversionRequest
    {
        public string FromCurrency { get; set; }
        public string ToCurrency { get; set; }
        public decimal Amount { get; set; }
    }
}
