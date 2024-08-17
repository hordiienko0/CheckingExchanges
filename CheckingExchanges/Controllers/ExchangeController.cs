﻿using Application.Interfaces;
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
            //var validationResult = await _estimateValidator.ValidateAsync(request);
            //if (!validationResult.IsValid)
            //{
            //    return BadRequest(validationResult.Errors);
            //}

            var result = await _exchangeService.GetBestRateAsync(request.InputAmount, request.InputCurrency, request.OutputCurrency);
            return Ok(result);
        }

        [HttpGet("getRates")]
        public async Task<IActionResult> GetRates([FromQuery] GetRatesRequest request)
        {
            //var validationResult = await _getRatesValidator.ValidateAsync(request);
            //if (!validationResult.IsValid)
            //{
            //    return BadRequest(validationResult.Errors);
            //}

            var result = await _exchangeService.GetRatesAsync(request.BaseCurrency, request.QuoteCurrency);
            return Ok(result);
        }
    }
}