namespace CurrencyConverter.Core.Exceptions
{
    public class BusinessException : Exception
    {
        public string Message { get; private set; }
        public BusinessException(string message) : base(message)
        {
            this.Message = message;
        }
    }
}
