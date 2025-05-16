using System.ComponentModel.DataAnnotations;
using CurrencyConverter.Core.Abstractions;
using CurrencyConverter.Core.Entities;
using CurrencyConverter.Core.Exceptions;
using CurrencyConverter.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CurrencyConverter.API.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/exchange-rates")]
    public class ExchangeRatesController : ControllerBase
    {
        private readonly ExchangeRateService _exchangeRateService;
        private readonly ILogger<ExchangeRatesController> _logger;

        public ExchangeRatesController(ExchangeRateService exchangeRateService, ILogger<ExchangeRatesController> logger)
        {
            _exchangeRateService = exchangeRateService;
            _logger = logger;
        }

        [HttpGet("latest")]
        public async Task<ActionResult<ExchangeRate>> GetLatestRates(
            [FromQuery, Required] string baseCurrency,
            CancellationToken cancellationToken)
        {
            ExchangeRate? exchangeRate = await _exchangeRateService.GetLatestRatesAsync(baseCurrency, cancellationToken);
            return Ok(exchangeRate);
        }

        [HttpGet("latest-range")]
        public async Task<ActionResult<ExchangeRateTimeSeries>> GetLatestRates(
            [FromQuery, Required] string baseCurrency,
            [FromQuery, Required] DateOnly startDate,
            [FromQuery, Required] DateOnly endDate,
            CancellationToken cancellationToken)
        {
            ExchangeRateTimeSeries exchangeRate = await _exchangeRateService.GetLatestRatesAsync(startDate, endDate, baseCurrency, cancellationToken);
            return Ok(exchangeRate);
        }

        [HttpPost("convert")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CurrencyConversionResult))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<CurrencyConversionResult>> ConvertCurrency(
            [FromBody] CurrencyConversionRequest request,
            CancellationToken cancellationToken)
        {
            if (request == null)
            {
                return BadRequest(new { error = "Request cannot be null" });
            }

            var result = await _exchangeRateService.ConvertCurrencyAsync(request, cancellationToken);
            return Ok(result);
        }
    }
}
