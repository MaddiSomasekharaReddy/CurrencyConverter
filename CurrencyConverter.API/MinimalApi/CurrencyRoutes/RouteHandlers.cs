using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CurrencyConverter.Model;
using CurrencyConverter.Model.Exchange;
using CurrencyConverter.Infrastructure;
using CurrencyConverter.Common;

namespace CurrencyConverter.Api.MinimalApi.CurrencyRoutes
{
    public static class RouteHandlers
    {
        public static void MapRoutes(this RouteGroupBuilder group)
        {
            // Latest Exchange Rates
            group.MapGet("/currency/rates/latest", GetLatestExchangeRates);

            // Currency Conversion
            group.MapPost("/currency/convert", ConvertCurrency);

            // Historical Exchange Rates
            group.MapGet("/currency/rates/historical", GetHistoricalRates);
        }

        /// <summary>
        /// get latest exchange rates - GET /api/v1/currency/rates/latest?baseCurrency=EUR
        /// </summary>
        /// <param name="baseCurrency"></param>
        /// <param name="providerFactory"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public static async Task<IResult> GetLatestExchangeRates(
            string baseCurrency,
            [FromServices] ICurrencyProviderFactory providerFactory,
            CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(baseCurrency))
                return Results.BadRequest("Base currency is required.");

            if (CurrencyRules.Excluded.Contains(baseCurrency))
                return Results.BadRequest($"Currency {baseCurrency} is not supported.");

            var provider = providerFactory.Resolve();
            var response = await provider.GetLatest(baseCurrency, ct);
            return Results.Ok(response);
        }

        /// <summary>
        /// Convert currency -  POST /api/v1/currency/convert  (body -> ConvertCurrencyRequest)
        /// </summary>
        /// <param name="request"></param>
        /// <param name="providerFactory"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public static async Task<IResult> ConvertCurrency(
            [FromBody] ConvertCurrencyRequest? request,
            [FromServices] ICurrencyProviderFactory providerFactory,
            CancellationToken ct)
        {
            if (request is null)
                return Results.BadRequest("Request body is required.");

            if (string.IsNullOrWhiteSpace(request.From) || string.IsNullOrWhiteSpace(request.To))
                return Results.BadRequest("Both 'From' and 'To' currencies are required.");

            if (CurrencyRules.Excluded.Contains(request.From) || CurrencyRules.Excluded.Contains(request.To))
                return Results.BadRequest($"Currency {request.From} or {request.To} is not supported.");

            var provider = providerFactory.Resolve();
            var result = await provider.Convert(request.From, request.To, request.Amount, ct);
            return Results.Ok(result);
        }

        /// <summary>
        /// Get historical exchange rates - GET /api/v1/currency/rates/historical?baseCurrency=EUR&startDate=2024-01-01&endDate=2024-01-31&page=1&pageSize=10
        /// </summary>
        /// <param name="baseCurrency"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="providerFactory"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public static async Task<IResult> GetHistoricalRates(
            string baseCurrency,
            DateTime startDate,
            DateTime endDate,
            int page,
            int pageSize,
            [FromServices] ICurrencyProviderFactory providerFactory,
            CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(baseCurrency))
                return Results.BadRequest("Base currency is required.");

            if (CurrencyRules.Excluded.Contains(baseCurrency))
                return Results.BadRequest($"Currency {baseCurrency} is not supported.");

            if (startDate > endDate)
                return Results.BadRequest("'startDate' must be <= 'endDate'.");

            if (page <= 0) page = 1;
            if (pageSize <= 0) pageSize = 10;
            pageSize = Math.Min(pageSize, 100);

            var provider = providerFactory.Resolve();
            var response = await provider.GetHistorical(baseCurrency, startDate, endDate, page, pageSize, ct);

            if (response?.Rates != null)
            {
                response.Rates = response.Rates
                    .Where(r => !CurrencyRules.Excluded.Contains(r.Currency, StringComparer.OrdinalIgnoreCase))
                    .ToList();
            }

            return Results.Ok(response);
        }
    }
}
