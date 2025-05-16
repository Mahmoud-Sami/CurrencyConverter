namespace CurrencyConverter.Core.Exceptions
{
    public class UnsupportedCurrencyException : BusinessException
    {
        public string Currency { get; set; }
        public UnsupportedCurrencyException(string currency)
        {
            this.Currency = currency;
        }
    }
}
