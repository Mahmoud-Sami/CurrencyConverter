namespace CurrencyConverter.Core.Exceptions
{
    public class UnsupportedCurrencyException : BusinessException
    {
        public string Currency { get; set; }
        public UnsupportedCurrencyException(string currency) : base($"The currency '{currency}' is not supported.")
        {
            this.Currency = currency;
        }
    }
}
