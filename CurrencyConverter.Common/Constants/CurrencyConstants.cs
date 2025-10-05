namespace CurrencyConverter.Common.Constants
{
    public static class CurrencyConstants
    {
        public static readonly HashSet<string> ExcludedCurrencies = new()
        {
            "TRY", "PLN", "THB", "MXN"
        };

        public const decimal MaxAmount = 1_000_000M;
        public const decimal MinAmount = 0.01M;
        public const decimal TransactionFeeRate = 0.01M;

        public const string FrankfurterProviderName = "Frankfurter";
        public const string DefaultProviderName = FrankfurterProviderName;
    }
}
