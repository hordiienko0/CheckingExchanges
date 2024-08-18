using Application.Interfaces;
using CheckingExchanges.Models;
using Domain.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace CheckingExchanges.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ExchangeController : ControllerBase
    {
        private readonly IExchangeService _exchangeService;

        public ExchangeController(IExchangeService exchangeService)
        {
            _exchangeService = exchangeService;
        }

        [HttpGet("estimate")]
        public async Task<IActionResult> Estimate([FromQuery] EstimateRequest request)
        {
            try
            {
                var result = await _exchangeService.EstimateExchangeAsync(request.InputAmount, request.InputCurrency, request.OutputCurrency);
                if (result.IsSuccess)
                {
                    return Ok(result.Value);
                }
                return BadRequest(result.Error);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("getRates")]
        public async Task<IActionResult> GetRates([FromQuery] GetRatesRequest request)
        {
            try
            {
                var result = await _exchangeService.GetRatesAsync(request.BaseCurrency, request.QuoteCurrency);
                if (result.IsSuccess)
                {
                    return Ok(result.Value);
                }
                return BadRequest(result.Error);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}