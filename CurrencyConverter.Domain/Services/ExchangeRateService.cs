using CurrencyConverter.Core.Abstractions;
using CurrencyConverter.Core.Entities;
using CurrencyConverter.Core.Exceptions;
using Microsoft.Extensions.Logging;

namespace CurrencyConverter.Core.Services
{
    public class ExchangeRateService
    {
        private readonly IExchangeRateProvider _exchangeRateProvider;
        private readonly ILogger<ExchangeRateService> _logger;
        public ExchangeRateService(IExchangeRateProvider exchangeRateProvider, ILogger<ExchangeRateService> logger)
        {
            _exchangeRateProvider = exchangeRateProvider;
            _logger = logger;
        }

        public async Task<ExchangeRate?> GetLatestRatesAsync(string baseCurrency, CancellationToken cancellationToken = default)
        {
            ExchangeRate? exchangeRate = await _exchangeRateProvider.GetLatestRatesAsync(baseCurrency, cancellationToken);
            return exchangeRate;
        }

        public async Task<ExchangeRateTimeSeries> GetLatestRatesAsync(DateOnly startDate, DateOnly endDate, string baseCurrency, CancellationToken cancellationToken = default)
        {
            ExchangeRateTimeSeries exchangeRate = await _exchangeRateProvider.GetHistoricalRatesAsync(startDate, endDate, baseCurrency, cancellationToken);
            return exchangeRate;
        }

        public async Task<CurrencyConversionResult> ConvertCurrencyAsync(
            CurrencyConversionRequest request,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation(
                "Converting {Amount} from {FromCurrency} to {ToCurrency}",
                request.Amount,
                request.FromCurrency,
                request.ToCurrency);

            ValidateCurrency(request.FromCurrency);
            ValidateCurrency(request.ToCurrency);

            var rates = await GetLatestRatesAsync(request.FromCurrency, cancellationToken);

            if (!rates.Rates.TryGetValue(request.ToCurrency, out var rate))
            {
                throw new UnsupportedCurrencyException(request.ToCurrency);
            }

            var convertedAmount = request.Amount * rate;

            return new CurrencyConversionResult
            {
                FromCurrency = request.FromCurrency,
                ToCurrency = request.ToCurrency,
                Amount = request.Amount,
                ConvertedAmount = convertedAmount,
                ExchangeRate = rate,
                Date = rates.Date
            };
        }

        private void ValidateCurrency(string currency)
        {
            if (string.IsNullOrWhiteSpace(currency))
                throw new ArgumentException("Currency code cannot be empty", nameof(currency));

            if (Enum.IsDefined(typeof(RestrictedCurrencies), currency))
                throw new RestrictedCurrencyException(currency);
        }
    }
}
