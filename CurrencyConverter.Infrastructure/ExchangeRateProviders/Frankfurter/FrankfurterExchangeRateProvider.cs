using System.Text.Json;
using CurrencyConverter.Core.Abstractions;
using CurrencyConverter.Core.Entities;
using Microsoft.Extensions.Logging;

namespace CurrencyConverter.Infrastructure.ExchangeRateProviders.Frankfurter
{
    public class FrankfurterExchangeRateProvider : IExchangeRateProvider
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<FrankfurterExchangeRateProvider> _logger;
        private const string BaseUrl = "https://api.frankfurter.dev";

        public string ProviderName => "Frankfurter";

        public FrankfurterExchangeRateProvider(HttpClient httpClient, ILogger<FrankfurterExchangeRateProvider> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<ExchangeRate?> GetLatestRatesAsync(string baseCurrency, CancellationToken cancellationToken = default)
        {
            try
            {
                HttpResponseMessage httpResponse = await _httpClient.GetAsync(BaseUrl + $"/v1/latest?base={baseCurrency}", cancellationToken);
                httpResponse.EnsureSuccessStatusCode();

                string responseBody = await httpResponse.Content.ReadAsStringAsync();

                FrankfurterResponse? response = JsonSerializer.Deserialize<FrankfurterResponse>(responseBody);

                ExchangeRate exchangeRate = new()
                {
                    BaseCurrency = response.Base,
                    Date = response.Date,
                    Rates = response.Rates
                };

                return exchangeRate;
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"Request error: {e.Message}");
                throw;
            }

        }

        public async Task<ExchangeRateTimeSeries> GetHistoricalRatesAsync(
            DateOnly startDate,
            DateOnly endDate,
            string baseCurrency,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation(
                "Requesting historical rates from Frankfurter API for period {StartDate} to {EndDate} with base currency {BaseCurrency}",
                startDate.ToString("yyyy-MM-dd"),
                endDate.ToString("yyyy-MM-dd"),
                baseCurrency);


            try
            {
                HttpResponseMessage httpResponse = await _httpClient.GetAsync(BaseUrl + $"/v1/{startDate:yyyy-MM-dd}..{endDate:yyyy-MM-dd}?from={baseCurrency}", cancellationToken);
                httpResponse.EnsureSuccessStatusCode();

                string responseBody = await httpResponse.Content.ReadAsStringAsync();

                FrankfurterTimeSeriesResponse? response = JsonSerializer.Deserialize<FrankfurterTimeSeriesResponse>(responseBody);

                ExchangeRateTimeSeries exchangeRate = new()
                {
                    Amount = response.Amount,
                    Base = response.Base,
                    StartDate = response.StartDate,
                    EndDate = response.EndDate,
                    Rates = response.Rates
                };

                return exchangeRate;
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"Request error: {e.Message}");
                throw;
            }

        }
    }
}
