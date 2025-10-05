using Microsoft.VisualStudio.TestTools.UnitTesting;
using CurrencyConverter.Infrastructure;
using CurrencyConverter.Infrastructure.Providers;
using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Caching.Distributed;
using Moq;

namespace CurrencyConverter.Test.Unit
{
    [TestClass]
    public class CurrencyProviderFactoryTests
    {
        [TestMethod]
        public void CurrencyProviderFactory_ResolvesFrankfurterProvider()
        {
            var services = new ServiceCollection();
            services.AddSingleton(new HttpClient());
            services.AddSingleton(new Mock<ILogger<FrankfurterProvider>>().Object);
            services.AddSingleton(new Mock<IDistributedCache>().Object);
            services.AddSingleton(Options.Create(new FrankfurterOptions()));
            services.AddScoped<ICurrencyProvider, FrankfurterProvider>();
            services.AddScoped<FrankfurterProvider>();
            var sp = services.BuildServiceProvider();
            var factory = new CurrencyProviderFactory(sp);
            var resolved = factory.Resolve("Frankfurter");
            Assert.IsNotNull(resolved);
            Assert.IsInstanceOfType(resolved, typeof(FrankfurterProvider));
        }
    }
}
