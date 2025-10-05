using CurrencyConverter.Common.Constants;

namespace CurrencyConverter.Domain
{
    public static class CurrencyBusinessRules
    {
        public static bool IsSupportedCurrency(string currency)
        {
            return currency != null && !CurrencyConstants.ExcludedCurrencies.Contains(currency);
        }

        public static bool IsValidAmount(decimal amount)
        {
            return amount >= CurrencyConstants.MinAmount && amount < CurrencyConstants.MaxAmount;
        }

        public static decimal CalculateTransactionFee(decimal amount)
        {
            return Math.Round(amount * CurrencyConstants.TransactionFeeRate, 2);
        }
    }
}
