using System;
using CurrencyConverter.Infrastructure.Providers;
using Microsoft.Extensions.DependencyInjection;
using CurrencyConverter.Common.Constants;

namespace CurrencyConverter.Infrastructure
{
    public class CurrencyProviderFactory : ICurrencyProviderFactory
    {
        private readonly IServiceProvider _serviceProvider;
        public CurrencyProviderFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        /// <summary>
        /// resolve provider by name
        /// </summary>
        /// <param name="providerName"></param>
        /// <returns></returns>
        /// <exception cref="NotSupportedException"></exception>
        public ICurrencyProvider Resolve(string providerName = CurrencyConstants.DefaultProviderName)
        {
            return providerName switch
            {
                CurrencyConstants.FrankfurterProviderName => _serviceProvider.GetRequiredService<FrankfurterProvider>(),
                _ => throw new NotSupportedException($"Provider '{providerName}' is not supported.")
            };
        }
    }
}
