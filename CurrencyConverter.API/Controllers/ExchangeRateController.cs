using System.ComponentModel.DataAnnotations;
using CurrencyConverter.Core.Abstractions;
using CurrencyConverter.Core.Entities;
using CurrencyConverter.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace CurrencyConverter.API.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/exchange-rates")]
    public class ExchangeRatesController : ControllerBase
    {
        private readonly ExchangeRateService _exchangeRateService;

        public ExchangeRatesController(ExchangeRateService exchangeRateService)
        {
            _exchangeRateService = exchangeRateService;
        }

        [HttpGet("latest")]
        public async Task<ActionResult<ExchangeRate>> GetLatestRates(
            [FromQuery, Required] string baseCurrency,
            CancellationToken cancellationToken)
        {
            ExchangeRate exchangeRate = await _exchangeRateService.GetLatestRatesAsync(baseCurrency, cancellationToken);
            return Ok(exchangeRate);
        }
    }
}
