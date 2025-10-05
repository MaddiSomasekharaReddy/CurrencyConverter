using System.Threading;
using System.Threading.Tasks;
using CurrencyConverter.Model;

namespace CurrencyConverter.Infrastructure
{
    public interface ICurrencyProviderFactory
    {
        ICurrencyProvider Resolve(string providerName = "Frankfurter");
    }
}
