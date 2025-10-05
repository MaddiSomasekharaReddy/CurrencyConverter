using Microsoft.VisualStudio.TestTools.UnitTesting;
using CurrencyConverter.Common.Constants;

namespace CurrencyConverter.Test.Unit
{
    [TestClass]
    public class CurrencyRulesTests
    {
        [TestMethod]
        public void ExcludedCurrencies_ShouldContainKnownCurrency()
        {
            Assert.IsTrue(CurrencyConstants.ExcludedCurrencies.Contains("TRY"));
            Assert.IsTrue(CurrencyConstants.ExcludedCurrencies.Contains("PLN"));
            Assert.IsTrue(CurrencyConstants.ExcludedCurrencies.Contains("THB"));
            Assert.IsTrue(CurrencyConstants.ExcludedCurrencies.Contains("MXN"));
        }
    }
}
