using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace CurrencyConverter.API.Extensions
{
    public static class OpenTelemetryExtensions
    {
        public static IServiceCollection AddCurrencyConverterOpenTelemetry(this IServiceCollection services)
        {
            services.AddOpenTelemetry()
                .WithTracing(tracerProviderBuilder =>
                {
                    tracerProviderBuilder
                        .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("CurrencyConverter.API"))
                        .AddAspNetCoreInstrumentation()
                        .AddHttpClientInstrumentation()
                        .AddConsoleExporter();
                });
            return services;
        }
    }
}
