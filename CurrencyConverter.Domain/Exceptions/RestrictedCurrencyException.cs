namespace CurrencyConverter.Core.Exceptions
{
    public class RestrictedCurrencyException : BusinessException
    {
        public string Currency { get; set; }
        public RestrictedCurrencyException(string currency)
        {
            this.Currency = currency;
        }
    }
}
