using Microsoft.AspNetCore.Authorization;

namespace CurrencyConverter.Api.MinimalApi.CurrencyRoutes
{
    public static class RouteHandlers
    {
        public static void MapRoutes(this RouteGroupBuilder group)
        {
            // ✅ Latest Exchange Rates
            group.MapGet("/api/v1/currency/rates/latest", GetLatestExchangeRates())
                 .WithName("GetLatestExchangeRates")
                 .RequireAuthorization()
                 .WithOpenApi();

            // ✅ Currency Conversion
            group.MapPost("/api/v1/currency/convert", ConvertCurrency())
                 .WithName("ConvertCurrency")
                 .RequireAuthorization(new AuthorizeAttribute { Roles = "Reader,Admin" })
                 .WithOpenApi();

            // ✅ Historical Exchange Rates
            group.MapGet("/api/v1/currency/rates/historical", GetHistoricalRates())
                 .WithName("GetHistoricalExchangeRates")
                 .RequireAuthorization()
                 .WithOpenApi();

        }

    }
}
