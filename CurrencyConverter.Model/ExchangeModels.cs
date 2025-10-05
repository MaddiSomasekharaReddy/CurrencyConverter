namespace CurrencyConverter.Model
{
    public class ConvertCurrencyRequest
    {
        public string From { get; set; } = null!;
        public string To { get; set; } = null!;
        public decimal Amount { get; set; }
    }
}

namespace CurrencyConverter.Model.Exchange
{
    public class ExchangeRateResponse
    {
        public string BaseCurrency { get; set; } = string.Empty;
        public Dictionary<string, decimal> Rates { get; set; } = new();
    }

    public class HistoricalRatesResponse
    {
        public string BaseCurrency { get; set; } = string.Empty;
        public List<RateEntry> Rates { get; set; } = new();
    }

    public class RateEntry
    {
        public string Currency { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public decimal Rate { get; set; }
    }
}
