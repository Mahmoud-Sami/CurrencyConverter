using CurrencyConverter.Core.Abstractions;
using CurrencyConverter.Core.Entities;

namespace CurrencyConverter.Core.Services
{
    public class ExchangeRateService
    {
        private readonly IExchangeRateProvider _exchangeRateProvider;
        public ExchangeRateService(IExchangeRateProvider exchangeRateProvider)
        {
            _exchangeRateProvider = exchangeRateProvider;
        }

        public async Task<ExchangeRate> GetLatestRatesAsync(string baseCurrency, CancellationToken cancellationToken = default)
        {
            ExchangeRate exchangeRate = await _exchangeRateProvider.GetLatestRatesAsync(baseCurrency, cancellationToken);
            return exchangeRate;
        }
    }
}
