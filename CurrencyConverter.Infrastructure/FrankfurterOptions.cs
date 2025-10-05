namespace CurrencyConverter.Infrastructure
{
    /// <summary>
    /// Frankfurter API options
    /// </summary>
    public class FrankfurterOptions
    {
        public string BaseUrl { get; set; } = null!;
        public int TimeoutSeconds { get; set; }
        public int CacheMinutes { get; set; }
    }
}
