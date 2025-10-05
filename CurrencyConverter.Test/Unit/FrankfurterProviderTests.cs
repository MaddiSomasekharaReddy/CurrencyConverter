using CurrencyConverter.Infrastructure;
using CurrencyConverter.Model;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Microsoft.Extensions.Caching.Distributed;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using CurrencyConverter.Infrastructure.Providers;
using System.Text.Json;

namespace CurrencyConverter.Test.Unit
{
    [TestClass]
    public class FrankfurterProviderTests
    {
        [TestMethod]
        public async Task GetLatest_ReturnsRates()
        {
            var httpClient = new HttpClient(new MockHttpMessageHandler());
            var logger = new Mock<ILogger<FrankfurterProvider>>().Object;
            var cacheMock = new Mock<IDistributedCache>();
            cacheMock.Setup(c => c.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync((byte[]?)null);
            var options = Options.Create(new FrankfurterOptions { BaseUrl = "https://api.frankfurter.dev/v1/", TimeoutSeconds = 10, CacheMinutes = 10 });
            var provider = new FrankfurterProvider(httpClient, logger, cacheMock.Object, options);
            var result = await provider.GetLatest("EUR", CancellationToken.None);
            Assert.IsNotNull(result);
            Assert.IsFalse(result.Rates.ContainsKey("TRY"));
            Assert.IsFalse(result.Rates.ContainsKey("PLN"));
            Assert.IsFalse(result.Rates.ContainsKey("THB"));
            Assert.IsFalse(result.Rates.ContainsKey("MXN"));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public async Task GetLatest_ExcludedCurrency_Throws()
        {
            var httpClient = new HttpClient(new MockHttpMessageHandler());
            var logger = new Mock<ILogger<FrankfurterProvider>>().Object;
            var cacheMock = new Mock<IDistributedCache>();
            cacheMock.Setup(c => c.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync((byte[]?)null);
            var options = Options.Create(new FrankfurterOptions { BaseUrl = "https://api.frankfurter.dev/v1/", TimeoutSeconds = 10, CacheMinutes = 10 });
            var provider = new FrankfurterProvider(httpClient, logger, cacheMock.Object, options);
            await provider.GetLatest("TRY", CancellationToken.None);
        }

        private class MockHttpMessageHandler : HttpMessageHandler
        {
            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                var response = new HttpResponseMessage(System.Net.HttpStatusCode.OK)
                {
                    Content = new StringContent("{\"base\":\"EUR\",\"rates\":{\"USD\":1.1,\"TRY\":30,\"PLN\":4.2,\"THB\":35,\"MXN\":18.5}}")
                };
                return Task.FromResult(response);
            }
        }

        [TestMethod]
        public void FrankfurterProvider_CanBeConstructed()
        {
            var httpClient = new HttpClient();
            var logger = new Mock<ILogger<FrankfurterProvider>>().Object;
            var cache = new Mock<IDistributedCache>();
            var options = Options.Create(new FrankfurterOptions());
            var provider = new FrankfurterProvider(httpClient, logger, cache.Object, options);
            Assert.IsNotNull(provider);
        }
    }
}
