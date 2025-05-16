namespace CurrencyConverter.Core.Exceptions
{
    public class RestrictedCurrencyException : BusinessException
    {
        public string Currency { get; set; }
        public RestrictedCurrencyException(string currency) : base($"The currency '{currency}' is restricted and cannot be used for conversion.")
        {
            this.Currency = currency;
        }
    }
}
