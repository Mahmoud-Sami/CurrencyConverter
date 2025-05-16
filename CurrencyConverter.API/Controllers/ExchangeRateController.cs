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
            ExchangeRate exchangeRate = await _exchangeRateService.GetLatestRatesAsync(baseCurrency, cancellationToken);
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
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<CurrencyConversionResult>> ConvertCurrency(
            [FromBody] CurrencyConversionRequest request,
            CancellationToken cancellationToken)
        {
            if (request == null)
            {
                return BadRequest(new { error = "Request cannot be null" });
            }

            try
            {
                var result = await _exchangeRateService.ConvertCurrencyAsync(request, cancellationToken);
                return Ok(result);
            }
            catch (RestrictedCurrencyException ex)
            {
                _logger.LogWarning("Restricted currency attempted: {Currency}", ex.Currency);
                return BadRequest(new { error = ex.Message });
            }
            catch (UnsupportedCurrencyException ex)
            {
                _logger.LogWarning("Unsupported currency: {Currency}", ex.Currency);
                return BadRequest(new { error = ex.Message });
            }
            //catch (ExchangeRateProviderUnavailableException ex)
            //{
            //    _logger.LogError("Provider unavailable: {ProviderName}", ex.ProviderName);
            //    return StatusCode(StatusCodes.Status503ServiceUnavailable, new { error = ex.Message });
            //}
            //catch (ExchangeRateRetrievalException ex)
            //{
            //    _logger.LogError(ex, "Error retrieving exchange rates for conversion");
            //    return StatusCode(StatusCodes.Status500InternalServerError, new { error = "Failed to perform currency conversion" });
            //}
        }
    }
}
