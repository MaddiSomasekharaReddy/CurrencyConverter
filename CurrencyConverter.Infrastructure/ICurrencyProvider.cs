using System;
using System.Threading;
using System.Threading.Tasks;
using CurrencyConverter.Model.Exchange;

namespace CurrencyConverter.Infrastructure
{
    public interface ICurrencyProvider
    {
        Task<ExchangeRateResponse> GetLatest(string baseCurrency, CancellationToken ct);
        Task<object> Convert(string from, string to, decimal amount, CancellationToken ct);
        Task<HistoricalRatesResponse> GetHistorical(string baseCurrency, DateTime startDate, DateTime endDate, int page, int pageSize, CancellationToken ct);
    }
}
