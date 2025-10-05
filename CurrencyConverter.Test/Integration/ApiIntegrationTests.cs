using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using CurrencyConverter.API;
using System.Text;
using System.Net.Http.Json;

namespace CurrencyConverter.Test.Integration
{
    [TestClass]
    public class ApiIntegrationTests
    {
        private static WebApplicationFactory<Program> _factory;
        private static HttpClient _client;

        [ClassInitialize]
        public static void ClassInit(TestContext context)
        {
            _factory = new WebApplicationFactory<Program>();
            _client = _factory.CreateClient();
        }

        [TestMethod]
        public async Task GetLatestRates_ReturnsSuccess()
        {
            var response = await _client.GetAsync("/currency/rates/latest?baseCurrency=EUR");
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }

        [TestMethod]
        public async Task ConvertCurrency_ReturnsSuccess()
        {
            var request = new
            {
                From = "EUR",
                To = "USD",
                Amount = 100
            };
            var response = await _client.PostAsJsonAsync("/currency/convert", request);
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }

        [TestMethod]
        public async Task GetHistoricalRates_ReturnsSuccess()
        {
            var response = await _client.GetAsync("/currency/rates/historical?baseCurrency=EUR&startDate=2024-01-01&endDate=2024-01-31&page=1&pageSize=10");
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }
    }
}
