using CurrencyConverter.Domain;
using CurrencyConverter.Model;
using CurrencyConverter.Model.Exchange;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using Polly.CircuitBreaker;
using Polly.Retry;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using CurrencyConverter.Model.Responses;
using CurrencyConverter.Common;

namespace CurrencyConverter.Infrastructure.Providers
{
    public class FrankfurterProvider : ICurrencyProvider
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<FrankfurterProvider> _logger;
        private readonly IDistributedCache _cache;
        private readonly FrankfurterOptions _options;
        private readonly AsyncRetryPolicy _retryPolicy;
        private readonly AsyncCircuitBreakerPolicy _circuitBreakerPolicy;

        public FrankfurterProvider(HttpClient httpClient, ILogger<FrankfurterProvider> logger, IDistributedCache cache, IOptions<FrankfurterOptions> options)
        {
            _httpClient = httpClient;
            _logger = logger;
            _cache = cache;
            _options = options.Value;

            _retryPolicy = Policy
                .Handle<HttpRequestException>()
                .Or<TaskCanceledException>()
                .WaitAndRetryAsync(3, attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)), (ex, ts, ctx) =>
                {
                    _logger.LogWarning(ex, "Retrying Frankfurter API after {Delay}s", ts.TotalSeconds);
                });

            _circuitBreakerPolicy = Policy
                .Handle<HttpRequestException>()
                .Or<TaskCanceledException>()
                .CircuitBreakerAsync(5, TimeSpan.FromMinutes(1),
                    onBreak: (ex, ts) => _logger.LogError(ex, "Frankfurter API circuit broken for {Duration}s", ts.TotalSeconds),
                    onReset: () => _logger.LogInformation("Frankfurter API circuit reset"));
        }

        public async Task<ExchangeRateResponse> GetLatest(string baseCurrency, CancellationToken ct)
        {
            if (!CurrencyBusinessRules.IsSupportedCurrency(baseCurrency))
                throw new ArgumentException($"Currency {baseCurrency} is not supported.");

            string cacheKey = $"latest:{baseCurrency}";
            var cached = await _cache.GetStringAsync(cacheKey, ct);
            if (cached != null)
            {
                return JsonSerializer.Deserialize<ExchangeRateResponse>(cached)!;
            }

            var url = $"{_options.BaseUrl}latest?base={baseCurrency}";
            var frankfurterResponse = await _retryPolicy.WrapAsync(_circuitBreakerPolicy)
                .ExecuteAsync(async () => await _httpClient.GetFromJsonAsync<FrankfurterLatestResponse>(url, ct));

            var filteredRates = frankfurterResponse.Rates
                .Where(kvp => CurrencyBusinessRules.IsSupportedCurrency(kvp.Key))
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            var result = new ExchangeRateResponse
            {
                BaseCurrency = frankfurterResponse.Base,
                Rates = filteredRates
            };

            await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(result), new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(_options.CacheMinutes) }, ct);
            return result;
        }

        public async Task<object> Convert(string from, string to, decimal amount, CancellationToken ct)
        {
            if (!CurrencyBusinessRules.IsSupportedCurrency(from) || !CurrencyBusinessRules.IsSupportedCurrency(to))
                throw new ArgumentException($"Currency {from} or {to} is not supported.");
            if (!CurrencyBusinessRules.IsValidAmount(amount))
                throw new ArgumentException($"Amount {amount} is not valid.");

            var url = $"{_options.BaseUrl}latest?amount={amount}&from={from}&to={to}";
            var frankfurterResponse = await _retryPolicy.WrapAsync(_circuitBreakerPolicy)
                .ExecuteAsync(async () => await _httpClient.GetFromJsonAsync<FrankfurterLatestResponse>(url, ct));

            var filteredRates = frankfurterResponse?.Rates
                .Where(kvp => CurrencyBusinessRules.IsSupportedCurrency(kvp.Key))
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            var fee = CurrencyBusinessRules.CalculateTransactionFee(amount);

            return new
            {
                From = from,
                To = to,
                Amount = amount,
                Fee = fee,
                Rates = filteredRates
            };
        }

        public async Task<HistoricalRatesResponse> GetHistorical(string baseCurrency, DateTime startDate, DateTime endDate, int page, int pageSize, CancellationToken ct)
        {
            if (!CurrencyBusinessRules.IsSupportedCurrency(baseCurrency))
                throw new ArgumentException($"Currency {baseCurrency} is not supported.");

            var url = $"{_options.BaseUrl}{startDate:yyyy-MM-dd}..{endDate:yyyy-MM-dd}?base={baseCurrency}";
            var frankfurterResponse = await _retryPolicy.WrapAsync(_circuitBreakerPolicy)
                .ExecuteAsync(async () => await _httpClient.GetFromJsonAsync<FrankfurterHistoricalResponse>(url, ct));

            var rates = new List<RateEntry>();
            foreach (var kvp in frankfurterResponse.Rates)
            {
                foreach (var currency in kvp.Value.Keys)
                {
                    if (CurrencyBusinessRules.IsSupportedCurrency(currency))
                    {
                        rates.Add(new RateEntry
                        {
                            Currency = currency,
                            Date = DateTime.Parse(kvp.Key),
                            Rate = kvp.Value[currency]
                        });
                    }
                }
            }
            var pagedRates = rates.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            return new HistoricalRatesResponse
            {
                BaseCurrency = frankfurterResponse.Base,
                Rates = pagedRates
            };
        }
    }
}
