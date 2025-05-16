using CurrencyConverter.Core.Entities;

namespace CurrencyConverter.Core.Abstractions
{
    public interface IExchangeRateProvider
    {
        string ProviderName { get; }
        Task<ExchangeRate?> GetLatestRatesAsync(string baseCurrency, CancellationToken cancellationToken = default);

        Task<ExchangeRateTimeSeries> GetHistoricalRatesAsync(DateOnly startDate, DateOnly endDate, string baseCurrency, CancellationToken cancellationToken = default);
    }
}
