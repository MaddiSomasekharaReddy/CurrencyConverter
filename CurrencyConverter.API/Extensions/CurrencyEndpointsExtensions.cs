using CurrencyConverter.Api.MinimalApi.CurrencyRoutes;

namespace CurrencyConverter.Api.Extensions
{
    public static class CurrencyEndpointsExtensions
    {
        public static WebApplication MapCurrencyEndpoints(this WebApplication app, string prefix = "/api/v1")
        {
            var group = app.MapGroup(prefix).WithTags("Currency");
            group.MapRoutes();
            return app;
        }
    }
}
